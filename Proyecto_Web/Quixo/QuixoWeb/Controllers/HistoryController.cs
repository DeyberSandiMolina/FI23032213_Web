using Microsoft.AspNetCore.Mvc;
using QuixoWeb.Data.Repositories;
using QuixoWeb.Application.Services;
using QuixoWeb.Application.DTOs;
using QuixoWeb.Domain;
using QuixoWeb.Models;
using System.Xml.Linq;

namespace QuixoWeb.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IQuixoRepository _repo;
        private readonly GameService _gameService;

        public HistoryController(IQuixoRepository repo, GameService gameService)
        {
            _repo = repo;
            _gameService = gameService;
        }

        // Página principal: lista de partidas
        public async Task<IActionResult> Index()
        {
            var games = await _repo.GetAllGamesAsync();
            return View(games);
        }

        // Página de reproducción de una partida específica
        public async Task<IActionResult> ViewGame(int id)
        {
            var game = await _repo.GetGameWithHistoryAsync(id);
            if (game == null)
                return NotFound();
            
            // Cargar todos los movimientos
            var moves = await GetMovesByGameIdAsync(id);
            
            // Reconstruir estados del juego
            var gameStates = await ReconstructGameStatesAsync(game, moves);
            
            ViewBag.GameId = id;
            ViewBag.Game = game;
            ViewBag.GameStates = gameStates;
            ViewBag.CurrentStateIndex = 0;
            ViewBag.TotalStates = gameStates.Count;
            ViewBag.IsFourPlayerMode = game.Mode == 4;
            ViewBag.CurrentMove = null;
            
            return View(gameStates[0]);
        }

        // GET: /History/ViewGameState/5?stateIndex=2
        public async Task<IActionResult> ViewGameState(int id, int stateIndex = 0)
        {
            var game = await _repo.GetGameWithHistoryAsync(id);
            if (game == null)
                return NotFound();

            // Cargar movimientos
            var moves = await GetMovesByGameIdAsync(id);
            
            // Reconstruir estados
            var gameStates = await ReconstructGameStatesAsync(game, moves);
            
            // Validar índice
            if (stateIndex < 0 || stateIndex >= gameStates.Count)
                stateIndex = 0;
            
            ViewBag.GameId = id;
            ViewBag.Game = game;
            ViewBag.GameStates = gameStates;
            ViewBag.CurrentStateIndex = stateIndex;
            ViewBag.TotalStates = gameStates.Count;
            ViewBag.IsFourPlayerMode = game.Mode == 4;
            
            // Obtener movimiento actual (si hay)
            ViewBag.CurrentMove = stateIndex > 0 ? 
                moves.FirstOrDefault(m => m.TurnNumber == stateIndex) : null;
            
            return View("ViewGame", gameStates[stateIndex]);
        }

        // Método para exportar a XML
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Export(int id)
        {
            var game = await _repo.GetGameWithHistoryAsync(id);
            if (game == null) return NotFound();
            
            // Cargar movimientos
            var moves = await GetMovesByGameIdAsync(id);
            
            // Crear documento XML
            var xmlDoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("QuixoGame",
                    new XElement("GameId", game.GameId),
                    new XElement("Mode", game.Mode == 4 ? "4Jugadores" : "2Jugadores"),
                    new XElement("CreatedAt", game.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("EndedAt", game.EndedAt?.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("Duration", game.TotalTime.ToString(@"hh\:mm\:ss")),
                    new XElement("Winner",
                        new XElement("PlayerId", game.WinnerPlayerId),
                        new XElement("TeamId", game.WinnerTeamId)
                    ),
                    new XElement("Players",
                        game.Players?.Select(p => new XElement("Player",
                            new XElement("Id", p.PlayerId),
                            new XElement("Name", p.Name)
                        )) ?? Enumerable.Empty<XElement>()
                    ),
                    new XElement("Moves",
                        moves.OrderBy(m => m.TurnNumber).Select(m => 
                            new XElement("Move",
                                new XElement("TurnNumber", m.TurnNumber),
                                new XElement("Symbol", m.Symbol.ToString()),
                                new XElement("From", $"{m.CubeTakenRow},{m.CubeTakenCol}"),
                                new XElement("To", $"{m.CubePlacedRow},{m.CubePlacedCol}"),
                                new XElement("Orientation", m.DotDirection)
                            )
                        )
                    )
                )
            );
            
            // Devolver archivo
            var fileName = $"quixo_game_{game.GameId}_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
            var memoryStream = new System.IO.MemoryStream();
            var writer = new System.IO.StreamWriter(memoryStream, System.Text.Encoding.UTF8);
            xmlDoc.Save(writer);
            writer.Flush();
            memoryStream.Position = 0;
            
            return File(memoryStream, "application/xml", fileName);
        }

        // ===== MÉTODOS PRIVADOS =====

        private async Task<List<Move>> GetMovesByGameIdAsync(int gameId)
        {
            var game = await _repo.GetGameWithHistoryAsync(gameId);
            return game?.Moves?.OrderBy(m => m.TurnNumber).ToList() ?? new List<Move>();
        }

        private async Task<List<GameStateDto>> ReconstructGameStatesAsync(Game game, List<Move> moves)
        {
            var gameStates = new List<GameStateDto>();
            var engine = new GameEngine(game.Mode == 4);

            // Estado inicial (turno 0)
            gameStates.Add(new GameStateDto
            {
                Board = MapGridToDto(engine.Board.Grid),
                CurrentSymbol = (int)engine.CurrentSymbolPublic,
                CurrentPlayer = engine.CurrentPlayerPublic,
                TurnNumber = 0,
                IsGameOver = false,
                IsFourPlayerMode = game.Mode == 4
            });

            // Aplicar cada movimiento en orden
            foreach (var move in moves.OrderBy(m => m.TurnNumber))
            {
                try
                {
                    engine.MakeMove(
                        move.CubeTakenRow,
                        move.CubeTakenCol,
                        move.CubePlacedRow,
                        move.CubePlacedCol,
                        move.DotDirection
                    );
                }
                catch (Exception ex)
                {
                    // Log error pero continuar
                    Console.WriteLine($"Error reconstruyendo movimiento {move.TurnNumber}: {ex.Message}");
                }

                gameStates.Add(new GameStateDto
                {
                    Board = MapGridToDto(engine.Board.Grid),
                    CurrentSymbol = (int)engine.CurrentSymbolPublic,
                    CurrentPlayer = engine.CurrentPlayerPublic,
                    TurnNumber = engine.TurnNumberPublic,
                    IsGameOver = engine.IsGameOverPublic,
                    WinnerPlayer = engine.WinnerPlayerPublic,
                    IsFourPlayerMode = game.Mode == 4
                });
            }

            return gameStates;
        }

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
    }
}