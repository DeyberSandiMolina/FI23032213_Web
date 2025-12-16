namespace QuixoWeb.Application.DTOs
{
    public class TeamStatsDto
    {
        public int TeamId { get; set; }
        public string Name { get; set; } = string.Empty; // Inicializar
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public double Effectiveness => GamesPlayed > 0 ? (GamesWon * 100.0) / GamesPlayed : 0;
    }
}