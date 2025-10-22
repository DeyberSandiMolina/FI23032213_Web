using System.ComponentModel.DataAnnotations;

namespace MVC.Models;

public class TheModel
{
    [Required(ErrorMessage = "La frase es requerida")]
    [StringLength(25, MinimumLength = 5, ErrorMessage = "La frase debe tener entre 5 y 25 caracteres")]
    public string? Phrase { get; set; }

    public Dictionary<char, int> Counts { get; set; } = [];

    public string? Lower { get; set; }

    public string? Upper { get; set; }
}
