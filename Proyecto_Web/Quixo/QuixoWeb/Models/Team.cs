using System.ComponentModel.DataAnnotations;

namespace QuixoWeb.Models
{
public class Team
{
    public int TeamId { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public int GameId { get; set; }

    // Jugadores pertenecientes al equipo
    public int Player1Id { get; set; }
    public int Player2Id { get; set; }

    public Player? Player1 { get; set; }
    public Player? Player2 { get; set; }

    // Estadísticas
    public int GamesWon { get; set; }
}

}
