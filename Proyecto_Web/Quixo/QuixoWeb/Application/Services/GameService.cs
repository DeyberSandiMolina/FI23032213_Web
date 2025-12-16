using System.Text.Json;
using QuixoWeb.Application.DTOs;
using QuixoWeb.Domain;
using QuixoWeb.Models;
using QuixoWeb.Data.Repositories;

namespace QuixoWeb.Application.Services
{
    public class GameService
    {
        private readonly IQuixoRepository _repo;

        // Juegos activos en memoria
        private readonly Dictionary<int, GameEngine> _activeGames = new();
        private readonly Dictionary<int, DateTime> _startTimes = new();

        public GameService(IQuixoRepository repo)
        {
            _repo = repo;
        }

        // ----------------------------------------------------------------------------------------
        // CREAR PARTIDA
        // ----------------------------------------------------------------------------------------
        public async Task<int> CreateGameAsync(bool mode4players)
        {
            Console.WriteLine($"GameService.CreateGameAsync - Creando juego modo 4 jugadores: {mode4players}");
            
            var engine = new GameEngine(mode4players);

            // Crear el juego con jugadores incluidos
            var game = new Game
            {
                CreatedAt = DateTime.UtcNow,
                Mode = mode4players ? 4 : 2,
                Players = new List<Player>()
            };

            // Crear jugadores según el modo
            int numPlayers = mode4players ? 4 : 2;
            for (int i = 1; i <= numPlayers; i++)
            {
                game.Players.Add(new Player 
                { 
                    Name = $"Jugador {i}"
                    // GameId se asignará automáticamente por EF Core
                });
            }

            // Guardar juego con jugadores (CASCADE save)
            var created = await _repo.CreateGameAsync(game);
            int gameId = created.GameId;

            Console.WriteLine($"Juego {gameId} creado con {numPlayers} jugadores:");
            foreach (var player in created.Players)
            {
                Console.WriteLine($"  - {player.Name} (ID: {player.PlayerId})");
            }

            // Si es modo 4 jugadores, crear equipos
            if (mode4players && created.Players.Count >= 4)
            {
                var teamA = new Team
                {
                    Name = "Equipo A",
                    GameId = gameId,
                    Player1Id = created.Players[0].PlayerId,
                    Player2Id = created.Players[2].PlayerId
                };
                
                var teamB = new Team
                {
                    Name = "Equipo B",
                    GameId = gameId,
                    Player1Id = created.Players[1].PlayerId,
                    Player2Id = created.Players[3].PlayerId
                };
                
                await _repo.CreateTeamsAsync(new List<Team> { teamA, teamB });
                Console.WriteLine("Equipos creados para modo 4 jugadores");
            }

            _activeGames[gameId] = engine;
            _startTimes[gameId] = DateTime.UtcNow;

            return gameId;
        }

        // ----------------------------------------------------------------------------------------
        // MAPEO DEL TABLERO
        // ----------------------------------------------------------------------------------------
        private List<BoardRowDto> MapGridToDto(Cube[,] grid)
        {
            var rows = new List<BoardRowDto>();
            int rowsCount = grid.GetLength(0);
            int colsCount = grid.GetLength(1);

            for (int r = 0; r < rowsCount; r++)
            {
                var rowDto = new BoardRowDto();
                for (int c = 0; c < colsCount; c++)
                {
                    var cube = grid[r, c];
                    rowDto.Cells.Add(new CubeDto
                    {
                        Symbol = (int)cube.Symbol,
                        Orientation = (int)cube.Orientation
                    });
                }
                rows.Add(rowDto);
            }
            return rows;
        }

        // ----------------------------------------------------------------------------------------
        // OBTENER ESTADO PARA LA UI
        // ----------------------------------------------------------------------------------------
        public async Task<GameStateDto> GetStateAsync(int gameId)
        {
            Console.WriteLine($"GameService.GetStateAsync - Solicitando estado del juego {gameId}");
            
            if (!_activeGames.ContainsKey(gameId))
            {
                Console.WriteLine($"GameService.GetStateAsync - Juego {gameId} no está en memoria. Cargando desde DB...");
                
                var game = await _repo.GetGameWithHistoryAsync(gameId);

                if (game == null)
                {
                    Console.WriteLine($"GameService.GetStateAsync - ERROR: Juego {gameId} no existe en DB");
                    throw new Exception("Juego no existe.");
                }

                var engine = new GameEngine(game.Mode == 4);

                // Reproducir movimientos uno por uno
                foreach (var move in game.Moves.OrderBy(m => m.TurnNumber))
                {
                    engine.MakeMove(
                        move.CubeTakenRow,
                        move.CubeTakenCol,
                        move.CubePlacedRow,
                        move.CubePlacedCol,
                        move.DotDirection
                    );
                }

                _activeGames[gameId] = engine;
                _startTimes[gameId] = game.CreatedAt;
                
                Console.WriteLine($"GameService.GetStateAsync - Juego {gameId} cargado desde DB. Turno: {engine.TurnNumberPublic}");
            }

            var eng = _activeGames[gameId];
            
            Console.WriteLine($"GameService.GetStateAsync - Devolviendo estado del juego {gameId}");

            return new GameStateDto
            {
                Board = MapGridToDto(eng.Board.Grid),
                CurrentSymbol = (int)eng.CurrentSymbolPublic,
                CurrentPlayer = eng.CurrentPlayerPublic,
                TurnNumber = eng.TurnNumberPublic,
                IsGameOver = eng.IsGameOverPublic,
                WinnerPlayer = eng.WinnerPlayerPublic,
                IsFourPlayerMode = eng.IsFourPlayerMode
            };
        }

        // ----------------------------------------------------------------------------------------
        // REALIZAR MOVIMIENTO - VERSIÓN CORREGIDA (SIN ERROR DE VARIABLE DUPLICADA)
        // ----------------------------------------------------------------------------------------
        public async Task<MoveResponseDto> MakeMoveAsync(int gameId, MoveRequestDto request)
        {
            Console.WriteLine($"GameService.MakeMoveAsync - Iniciando movimiento para juego {gameId}");
            Console.WriteLine($"  Movimiento: ({request.TakeRow},{request.TakeCol}) -> ({request.PlaceRow},{request.PlaceCol})");
            Console.WriteLine($"  Orientation: {request.Orientation}");
            Console.WriteLine($"  Juegos en memoria: {_activeGames.Count}");
            
            GameEngine engine;
            
            // Si no está en memoria, cargarlo desde la base de datos
            if (!_activeGames.ContainsKey(gameId))
            {
                Console.WriteLine($"GameService.MakeMoveAsync - Juego {gameId} no está en memoria. Cargando desde DB...");
                
                var game = await _repo.GetGameWithHistoryAsync(gameId);
                
                if (game == null)
                {
                    Console.WriteLine($"GameService.MakeMoveAsync - ERROR: Juego {gameId} no existe en DB");
                    return new MoveResponseDto
                    {
                        Success = false,
                        Message = "Juego no encontrado."
                    };
                }
                
                // Reconstruir el GameEngine desde la base de datos
                engine = new GameEngine(game.Mode == 4);
                
                // Reproducir todos los movimientos anteriores
                foreach (var move in game.Moves.OrderBy(m => m.TurnNumber))
                {
                    engine.MakeMove(
                        move.CubeTakenRow,
                        move.CubeTakenCol,
                        move.CubePlacedRow,
                        move.CubePlacedCol,
                        move.DotDirection
                    );
                }
                
                // Guardar en memoria
                _activeGames[gameId] = engine;
                _startTimes[gameId] = game.CreatedAt;
                
                Console.WriteLine($"GameService.MakeMoveAsync - Juego {gameId} cargado desde DB. Turno actual: {engine.TurnNumberPublic}");
            }
            else
            {
                // Ya está en memoria
                engine = _activeGames[gameId];
                Console.WriteLine($"GameService.MakeMoveAsync - Juego {gameId} ya está en memoria. Turno actual: {engine.TurnNumberPublic}");
            }
            
            Console.WriteLine($"GameService.MakeMoveAsync - Engine obtenido. Turno: {engine.TurnNumberPublic}, Jugador: {engine.CurrentPlayerPublic}");
            
            // **REMOVER ESTA SECCIÓN** - No verificar ganador ANTES del movimiento
            // Esto estaba causando problemas
            
            try
            {
                // Ejecutar movimiento con orientación
                Console.WriteLine($"GameService.MakeMoveAsync - Ejecutando movimiento en GameEngine...");
                var result = engine.MakeMove(
                    request.TakeRow,
                    request.TakeCol,
                    request.PlaceRow,
                    request.PlaceCol,
                    request.Orientation
                );
                
                Console.WriteLine($"GameService.MakeMoveAsync - Resultado del movimiento: {result}");
                Console.WriteLine($"GameService.MakeMoveAsync - ¿Juego terminado? {engine.IsGameOverPublic}");
                Console.WriteLine($"GameService.MakeMoveAsync - Ganador: {engine.WinnerPlayerPublic}");
                
                // Guardar movimiento en DB
                var boardDto = MapGridToDto(engine.Board.Grid);
                var move = new Move
                {
                    GameId = gameId,
                    PlayerId = null,
                    TurnNumber = engine.TurnNumberPublic,
                    CubeTakenRow = request.TakeRow,
                    CubeTakenCol = request.TakeCol,
                    CubePlacedRow = request.PlaceRow,
                    CubePlacedCol = request.PlaceCol,
                    Symbol = engine.CurrentSymbolPublic == CubeSymbol.Circle ? 'C' : 'X',
                    DotDirection = request.Orientation
                };
                
                Console.WriteLine($"GameService.MakeMoveAsync - Guardando movimiento en DB...");
                await _repo.SaveMoveAsync(move, JsonSerializer.Serialize(boardDto));
                Console.WriteLine($"GameService.MakeMoveAsync - Movimiento guardado en DB");
                
                // **SOLO AQUÍ** verificar si hay ganador y guardarlo
                if (engine.IsGameOverPublic)
                {
                    Console.WriteLine($"GameService.MakeMoveAsync - ¡HAY GANADOR! Jugador: {engine.WinnerPlayerPublic}");
                    
                    // Calcular duración del juego
                    TimeSpan duration = DateTime.UtcNow - _startTimes[gameId];
                    
                    // Determinar equipo ganador (solo para modo 4 jugadores)
                    int? winnerTeam = null;
                    if (engine.IsFourPlayerMode && engine.WinnerPlayerPublic.HasValue)
                    {
                        winnerTeam = (engine.WinnerPlayerPublic.Value == 1 || 
                                    engine.WinnerPlayerPublic.Value == 3) ? 1 : 2;
                    }
                    
                    
                    await _repo.EndGameAsync(
                        gameId,
                        engine.WinnerPlayerPublic.Value, 
                        winnerTeam,
                        duration
                    );
                    
                    Console.WriteLine($"Ganador guardado en DB: Jugador {engine.WinnerPlayerPublic.Value}");
                }
                
                return new MoveResponseDto
                {
                    Success = true,
                    Message = result switch
                    {
                        GameEngine.MoveResult.Valid => "Movimiento válido.",
                        GameEngine.MoveResult.Win => "¡Victoria!",
                        
                        GameEngine.MoveResult.LoseAccidental => "¡Has perdido por completar una línea para el oponente!",
                        _ => "Movimiento realizado."
                    },
                    Board = boardDto,
                    CurrentPlayer = engine.CurrentPlayerPublic,
                    CurrentSymbol = (int)engine.CurrentSymbolPublic,
                    IsGameOver = engine.IsGameOverPublic,
                    WinnerPlayer = engine.WinnerPlayerPublic
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GameService.MakeMoveAsync - EXCEPCIÓN: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                return new MoveResponseDto
                {
                    Success = false,
                    Message = ex.Message,
                    IsGameOver = engine.IsGameOverPublic,
                    WinnerPlayer = engine.WinnerPlayerPublic
                };
            }
        }
    }
}