using System.ComponentModel.DataAnnotations;

namespace QuixoWeb.Models


{
 public class Player
{
    public int PlayerId { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    // Modo de juego normal
    public List<Move> Moves { get; set; } = new();
}

}
