namespace QuixoWeb.Models
{
public class GameTeam
{
    public int GameTeamId { get; set; }

    public int GameId { get; set; }
    public int TeamId { get; set; }

    public Game? Game { get; set; }
    public Team? Team { get; set; }
}
}
