namespace QuixoWeb.Application.DTOs
{
    public class GameStateDto
    {
        public List<BoardRowDto> Board { get; set; } = new();
        public int CurrentPlayer { get; set; }
        public int CurrentSymbol { get; set; } // 1 = Circle, 2 = Cross
        public int TurnNumber { get; set; }

        public bool IsGameOver { get; set; } = false;
        public int? WinnerPlayer { get; set; } = null;
        public bool IsFourPlayerMode { get; set; }
    }
}
