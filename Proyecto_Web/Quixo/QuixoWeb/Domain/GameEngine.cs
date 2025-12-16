using System;
using System.Linq;

namespace QuixoWeb.Domain
{
    public class GameEngine
    {
        public Board Board { get; private set; } = new Board();
        public bool IsFourPlayerMode { get; private set; }
        
        private CubeSymbol CurrentSymbol;
        private int TurnNumber = 1;
        private int CurrentPlayer = 1;
        private bool GameOver = false;
        private int? WinnerPlayer = null;
        
        // Propiedades públicas
        public CubeSymbol CurrentSymbolPublic => CurrentSymbol;
        public int TurnNumberPublic => TurnNumber;
        public int CurrentPlayerPublic => CurrentPlayer;
        public bool IsGameOverPublic => GameOver;
        public int? WinnerPlayerPublic => WinnerPlayer;
        public int CurrentTeam => (CurrentPlayer == 1 || CurrentPlayer == 3) ? 1 : 2;

        public GameEngine(bool mode4players = false)
        {
            IsFourPlayerMode = mode4players;
            
            if (IsFourPlayerMode)
            {
                CurrentPlayer = 1;
                CurrentSymbol = CubeSymbol.Circle; // Equipo A empieza
            }
            else
            {
                CurrentSymbol = CubeSymbol.Circle; // Jugador 1 empieza
            }
        }

        // --- MÉTODO IsCorrectSymbol (modificado) ---
        public bool IsCorrectSymbol(int r, int c)
        {
            var cube = Board.Get(r, c);
            
            // Primera vuelta
            if (IsFirstRound())
                return cube.Symbol == CubeSymbol.Neutral;
            
            // Después de primera vuelta
            return cube.Symbol == CubeSymbol.Neutral || cube.Symbol == CurrentSymbol;
        }

        // --- MÉTODO IsFirstRound (SOLO UNO) ---
        private bool IsFirstRound()
        {
            if (IsFourPlayerMode)
                return TurnNumber <= 4; // 4 jugadores × 1 turno cada uno
            else
                return TurnNumber <= 2; // 2 jugadores × 1 turno cada uno
        }

        // --- TURNO SIGUIENTE ---
        private void NextTurn()
        {
            TurnNumber++;
            
            if (!IsFourPlayerMode)
            {
                // Modo 2 jugadores
                CurrentSymbol = (CurrentSymbol == CubeSymbol.Circle) 
                    ? CubeSymbol.Cross 
                    : CubeSymbol.Circle;
                return;
            }
            
            // Modo 4 jugadores
            CurrentPlayer++;
            if (CurrentPlayer > 4) CurrentPlayer = 1;
            
            // Determinar símbolo según equipo
            if (CurrentPlayer == 1 || CurrentPlayer == 3)
                CurrentSymbol = CubeSymbol.Circle; // Equipo A
            else
                CurrentSymbol = CubeSymbol.Cross; // Equipo B
        }

        // --- VALIDACIONES ESPECÍFICAS PARA 4 JUGADORES ---
        private bool CanPlayerTakeCube(int row, int col)
        {
            var cube = Board.Get(row, col);
            
            if (!IsFourPlayerMode)
            {
                // Modo 2 jugadores: reglas normales
                if (IsFirstRound())
                    return cube.Symbol == CubeSymbol.Neutral;
                
                return cube.Symbol == CubeSymbol.Neutral || cube.Symbol == CurrentSymbol;
            }
            
            // Modo 4 jugadores
            if (cube.Symbol == CubeSymbol.Neutral)
            {
                // Cubo neutro: solo en primera vuelta (4 turnos)
                return IsFirstRound();
            }
            
            // Cubo con símbolo: debe ser del equipo actual Y orientado hacia el jugador
            if (cube.Symbol != CurrentSymbol) return false;
            
            return IsCubeOrientedToPlayer(row, col, CurrentPlayer);
        }
        
        private bool IsCubeOrientedToPlayer(int row, int col, int player)
        {
            var cube = Board.Get(row, col);
            return (int)cube.Orientation == (player - 1);
        }

        // --- VALIDACIONES COMUNES ---
        public bool CanTake(int r, int c) => Board.IsPerimeter(r, c);
        public bool CanPlace(int r, int c) => Board.IsPerimeter(r, c);
        
        public bool CannotPlaceSamePosition(int takeR, int takeC, int placeR, int placeC)
            => !(takeR == placeR && takeC == placeC);
            
        public bool IsMovable(int takeR, int takeC, int placeR, int placeC)
            => takeR == placeR || takeC == placeC;

        // --- ORIENTACIÓN SEGÚN MOVIMIENTO ---
        private CubeOrientation CalculateOrientation(int placeR, int placeC, int? playerOrientation = null)
        {
            // Si el jugador especificó orientación (modo 4 jugadores), usarla
            if (playerOrientation.HasValue && IsFourPlayerMode)
            {
                return (CubeOrientation)playerOrientation.Value;
            }
            
            // Por defecto: orientar según posición de colocación
            if (placeR == 0) return CubeOrientation.Up;
            if (placeR == 4) return CubeOrientation.Down;
            if (placeC == 0) return CubeOrientation.Left;
            if (placeC == 4) return CubeOrientation.Right;
            
            return CubeOrientation.Up;
        }

        // --- DETECCIÓN DE GANADOR ---
        public CubeSymbol? CheckWinner()
        {
            Console.WriteLine("=== CheckWinner START ===");
            
            var b = Board.Grid;
            
            // Filas
            for (int r = 0; r < 5; r++)
            {
                if (SameSymbol(b[r, 0], b[r, 1], b[r, 2], b[r, 3], b[r, 4]))
                {
                    Console.WriteLine($"¡Línea horizontal en fila {r}! Símbolo: {b[r, 0].Symbol}");
                    return b[r, 0].Symbol;
                }
            }
            
            // Columnas
            for (int c = 0; c < 5; c++)
            {
                if (SameSymbol(b[0, c], b[1, c], b[2, c], b[3, c], b[4, c]))
                {
                    Console.WriteLine($"¡Línea vertical en columna {c}! Símbolo: {b[0, c].Symbol}");
                    return b[0, c].Symbol;
                }
            }
            
            // Diagonales
            if (SameSymbol(b[0, 0], b[1, 1], b[2, 2], b[3, 3], b[4, 4]))
            {
                Console.WriteLine($"¡Diagonal principal! Símbolo: {b[0, 0].Symbol}");
                return b[0, 0].Symbol;
            }
                
            if (SameSymbol(b[0, 4], b[1, 3], b[2, 2], b[3, 1], b[4, 0]))
            {
                Console.WriteLine($"¡Diagonal secundaria! Símbolo: {b[0, 4].Symbol}");
                return b[0, 4].Symbol;
            }
            
            Console.WriteLine("=== CheckWinner END - No hay ganador ===");
            return null;
        }
        
        private bool SameSymbol(params Cube[] cubes)
        {
            if (cubes.Any(c => c.Symbol == CubeSymbol.Neutral)) return false;
            return cubes.All(c => c.Symbol == cubes[0].Symbol);
        }

        private int GetPlayerBySymbol(CubeSymbol symbol)
        {
            if (!IsFourPlayerMode)
            {
                // Modo 2 jugadores
                // Jugador 1 = Circle (1), Jugador 2 = Cross (2)
                return symbol == CubeSymbol.Circle ? 1 : 2;
            }
            else
            {
                // Modo 4 jugadores
                if (symbol == CubeSymbol.Circle)
                {
                    // Equipo A gana (Jugadores 1 y 3)
                    // Retornamos el jugador del equipo A que NO está jugando actualmente
                    return (CurrentPlayer == 1 || CurrentPlayer == 3) ? 
                        (CurrentPlayer == 1 ? 3 : 1) : // Si el equipo A está jugando
                        (CurrentPlayer == 2 ? 1 : 3);  // Si el equipo B está jugando
                }
                else // Cross
                {
                    // Equipo B gana (Jugadores 2 y 4)
                    return (CurrentPlayer == 2 || CurrentPlayer == 4) ?
                        (CurrentPlayer == 2 ? 4 : 2) : // Si el equipo B está jugando
                        (CurrentPlayer == 1 ? 2 : 4);  // Si el equipo A está jugando
                }
            }
        }

        // --- ENUM DE RESULTADOS ---
        public enum MoveResult { Valid, Win, LoseAccidental }

        // --- MÉTODO PRINCIPAL MAKE MOVE ---
        public MoveResult MakeMove(int takeR, int takeC, int placeR, int placeC, int? orientation = null)
        {
            if (GameOver)
                throw new Exception("La partida ya finalizó.");
            
            // Validaciones básicas
            if (!CanTake(takeR, takeC))
                throw new Exception("Solo se puede tomar desde la periferia.");
                
            if (!CanPlayerTakeCube(takeR, takeC))
                throw new Exception("No puedes tomar este cubo.");
                
            if (!CanPlace(placeR, placeC))
                throw new Exception("Solo puedes colocar en la periferia.");
                
            if (!CannotPlaceSamePosition(takeR, takeC, placeR, placeC))
                throw new Exception("No puedes colocar en la misma posición.");
                
            if (!IsMovable(takeR, takeC, placeR, placeC))
                throw new Exception("El movimiento debe ser horizontal o vertical.");
            
            // Aplicar movimiento
            ApplyPush(takeR, takeC, placeR, placeC, orientation);
            
            // Verificar resultados
            var winner = CheckWinner();
            
            Console.WriteLine($"=== DETECCIÓN DE GANADOR ===");
            Console.WriteLine($"Winner from CheckWinner: {winner}");
            Console.WriteLine($"CurrentSymbol: {CurrentSymbol}");
            Console.WriteLine($"CurrentPlayer: {CurrentPlayer}");
            
            if (winner.HasValue)
            {
                GameOver = true;
                
                // En modo 2J: O = Jugador 1, X = Jugador 2
                if (!IsFourPlayerMode)
                {
                    WinnerPlayer = winner.Value == CubeSymbol.Circle ? 1 : 2;
                }
                else
                {
                    // Modo 4J: usa GetPlayerBySymbol
                    WinnerPlayer = GetPlayerBySymbol(winner.Value);
                }
                
                Console.WriteLine($"¡HAY GANADOR! Símbolo: {winner.Value}, Jugador: {WinnerPlayer}");
                
                if (winner.Value == CurrentSymbol)
                    return MoveResult.Win;
                else
                    return MoveResult.LoseAccidental;
            }
            
            NextTurn();
            return MoveResult.Valid;

            
            // Movimiento válido, siguiente turno
            NextTurn();
            return MoveResult.Valid;
        }

        // Nuevo método para obtener jugador por símbolo
        /*private int GetPlayerBySymbol(CubeSymbol symbol)
        {
            if (!IsFourPlayerMode)
            {
                // Modo 2 jugadores: simple
                return symbol == CubeSymbol.Circle ? 1 : 2;
            }
            
            // Modo 4 jugadores: determinar qué jugador del equipo ganador
            if (symbol == CubeSymbol.Circle)
            {
                // Equipo A (Jugadores 1 y 3) ganan
                // ¿Quién hizo el movimiento? El otro jugador gana
                return CurrentPlayer == 1 ? 3 : 1;
            }
            else // Cross
            {
                // Equipo B (Jugadores 2 y 4) ganan
                return CurrentPlayer == 2 ? 4 : 2;
            }
        }*/
        
        private void ApplyPush(int takeR, int takeC, int placeR, int placeC, int? orientation)
        {
            // Crear nuevo cubo con orientación
            var newCube = new Cube
            {
                Symbol = CurrentSymbol,
                Orientation = CalculateOrientation(placeR, placeC, orientation)
            };
            
            // Lógica de empuje
            if (takeR == placeR) // Movimiento horizontal
            {
                int row = takeR;
                
                if (placeC == 0) // Empujar hacia derecha
                {
                    for (int c = takeC; c > 0; c--)
                        Board.Grid[row, c] = Board.Grid[row, c - 1].Clone();
                    Board.Grid[row, 0] = newCube;
                }
                else if (placeC == 4) // Empujar hacia izquierda
                {
                    for (int c = takeC; c < 4; c++)
                        Board.Grid[row, c] = Board.Grid[row, c + 1].Clone();
                    Board.Grid[row, 4] = newCube;
                }
            }
            else if (takeC == placeC) // Movimiento vertical
            {
                int col = takeC;
                
                if (placeR == 0) // Empujar hacia abajo
                {
                    for (int r = takeR; r > 0; r--)
                        Board.Grid[r, col] = Board.Grid[r - 1, col].Clone();
                    Board.Grid[0, col] = newCube;
                }
                else if (placeR == 4) // Empujar hacia arriba
                {
                    for (int r = takeR; r < 4; r++)
                        Board.Grid[r, col] = Board.Grid[r + 1, col].Clone();
                    Board.Grid[4, col] = newCube;
                }
            }
        }
        
        private Cube[,] CopyGrid(Cube[,] src)
        {
            var dest = new Cube[5, 5];
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    dest[i, j] = src[i, j].Clone();
            return dest;
        }
    }
}