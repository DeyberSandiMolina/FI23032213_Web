namespace QuixoWeb.Models
{
    public class Move
    {
        public int MoveId { get; set; }
        public int GameId { get; set; }
        public int? PlayerId { get; set; }

        public int TurnNumber { get; set; }

        public int CubeTakenRow { get; set; }
        public int CubeTakenCol { get; set; }

        public int CubePlacedRow { get; set; }

        public int CubePlacedCol { get; set; }

        public char Symbol { get; set; }

        public int? DotDirection { get; set; }

        public Game? Game { get; set; }
        public Player? Player { get; set; }
    }
}
