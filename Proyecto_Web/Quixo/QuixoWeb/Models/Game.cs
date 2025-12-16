namespace QuixoWeb.Models
{
    public class Game
    {
        public int GameId { get; set; }

        // 2 = dos jugadores, 4 = cuatro jugadores
        public int Mode { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public TimeSpan TotalTime { get; set; }

        public int? WinnerPlayerId { get; set; }
        public int? WinnerTeamId { get; set; }

        public Player? WinnerPlayer { get; set; }
        public Team? WinnerTeam { get; set; }

        public List<Player> Players { get; set; } = new();
        public List<Team> Teams { get; set; } = new();

        public List<BoardState> BoardStates { get; set; } = new();
        public List<Move> Moves { get; set; } = new();
    }
}
