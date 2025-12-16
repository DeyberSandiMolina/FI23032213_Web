using Microsoft.EntityFrameworkCore;
using QuixoWeb.Data;
using QuixoWeb.Models;

namespace QuixoWeb.Data.Repositories
{
    public class QuixoRepository : IQuixoRepository
    {
        private readonly QuixoDbContext _context;

        public QuixoRepository(QuixoDbContext context)
        {
            _context = context;
        }

        public async Task<Game> CreateGameAsync(Game game, CancellationToken ct = default)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync(ct);
            
            // Cargar jugadores para asegurar que tienen IDs
            await _context.Entry(game)
                .Collection(g => g.Players)
                .LoadAsync(ct);
            
            return game;
        }

        public async Task CreateTeamsAsync(List<Team> teams)
        {
            _context.Teams.AddRange(teams);
            await _context.SaveChangesAsync();
        }

        public async Task SaveMoveAsync(Move move, string boardJson, CancellationToken ct = default)
        {
            // Guardar Move
            _context.Moves.Add(move);
            await _context.SaveChangesAsync(ct);

            // Crear BoardState asociado al Move
            var bs = new BoardState
            {
                GameId_FK = move.GameId,
                MoveId = move.MoveId,
                StateJson = boardJson
            };

            _context.BoardStates.Add(bs);
            await _context.SaveChangesAsync(ct);
        }

        public async Task EndGameAsync(int gameId, int? winnerPlayerId, int? winnerTeamId, TimeSpan totalTime, CancellationToken ct = default)
        {
            var game = await _context.Games.FindAsync(new object[] { gameId }, ct);
            if (game == null) return;

            game.EndedAt = DateTime.UtcNow;
            game.TotalTime = totalTime;
            game.WinnerPlayerId = winnerPlayerId;
            game.WinnerTeamId = winnerTeamId;

            _context.Games.Update(game);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<Game?> GetGameWithHistoryAsync(int gameId, CancellationToken ct = default)
        {
            return await _context.Games
                .Include(g => g.Moves.OrderBy(m => m.MoveId))
                .Include(g => g.BoardStates.OrderBy(bs => bs.MoveId))
                .Include(g => g.Players)
                .Include(g => g.Teams)
                .FirstOrDefaultAsync(g => g.GameId == gameId, ct);
        }

        public async Task<IEnumerable<Game>> GetAllGamesAsync(CancellationToken ct = default)
        {
            return await _context.Games
                .Include(g => g.Players)
                .Include(g => g.Teams)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync(ct);
        }

                public async Task<List<Game>> GetAllGamesAsync()
        {
            return await _context.Games
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<Game?> GetFullGameAsync(int gameId)
        {
            return await _context.Games
                .Include(g => g.Moves)
                .Include(g => g.BoardStates)
                .Include(g => g.WinnerPlayer)
                .Include(g => g.WinnerTeam)
                .FirstOrDefaultAsync(g => g.GameId == gameId);
        }

        public async Task<Game?> GetGameWithPlayersAsync(int gameId)
        {
            return await _context.Games
                .Include(g => g.Players)
                .FirstOrDefaultAsync(g => g.GameId == gameId);
        }

        public async Task<List<Game>> GetAllAsync()
        {
            return await _context.Games
                .Include(g => g.WinnerPlayer)
                .Include(g => g.WinnerTeam)
                .Include(g => g.BoardStates)
                .ToListAsync();
        }

        public async Task<List<Move>> GetMovesByGameIdAsync(int gameId)
        {
            return await _context.Moves
                .Where(m => m.GameId == gameId)
                .OrderBy(m => m.TurnNumber)
                .ToListAsync();
        }

        public async Task<Move?> GetMoveByTurnAsync(int gameId, int turnNumber)
        {
            return await _context.Moves
                .FirstOrDefaultAsync(m => m.GameId == gameId && m.TurnNumber == turnNumber);
        }


    }
}
