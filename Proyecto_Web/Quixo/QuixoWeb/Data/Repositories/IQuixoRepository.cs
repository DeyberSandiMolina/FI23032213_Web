using QuixoWeb.Models;

namespace QuixoWeb.Data.Repositories
{
    public interface IQuixoRepository
    {
        Task<List<Move>> GetMovesByGameIdAsync(int gameId); // Agregar este
        Task<Move?> GetMoveByTurnAsync(int gameId, int turnNumber); // Agregar este
        // Crea un juego nuevo y devuelve la entidad (con id)
        Task<Game> CreateGameAsync(Game game, CancellationToken ct = default);

        // Guarda el movimiento (y el boardState JSON asociado)
        Task SaveMoveAsync(Move move, string boardJson, CancellationToken ct = default);

        // Marca el juego como finalizado y guarda metadata (winner, EndedAt, TotalTime)
        Task EndGameAsync(int gameId, int? winnerPlayerId, int? winnerTeamId, TimeSpan totalTime, CancellationToken ct = default);

        // Recupera un juego junto con Moves y BoardStates (ordenados)
        Task<Game?> GetGameWithHistoryAsync(int gameId, CancellationToken ct = default);

        // Lista de juegos (para historial)
        Task<IEnumerable<Game>> GetAllGamesAsync(CancellationToken ct = default);
        
        Task<List<Game>> GetAllGamesAsync();
        Task<Game?> GetFullGameAsync(int gameId);
        Task<List<Game>> GetAllAsync();
        Task CreateTeamsAsync(List<Team> teams);

        Task<Game?> GetGameWithPlayersAsync(int gameId);

    }
}
