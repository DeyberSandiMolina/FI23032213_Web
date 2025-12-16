using QuixoWeb.Application.DTOs;
using QuixoWeb.Domain;
namespace QuixoWeb.Application.DTOs
{
public class MoveResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";

    // ➜ eliminar GameEngine.MoveResult
    public GameEngine.MoveResult Result { get; set; }

    // El board serializable
    public List<BoardRowDto> Board { get; set; } = new();

    // Estado para UI
    public int CurrentPlayer { get; set; }
    public int CurrentSymbol { get; set; }

    // Estado del juego
    public bool IsGameOver { get; set; } = false;
    public int? WinnerPlayer { get; set; } = null;
}
}
