using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuixoWeb.Data;
using QuixoWeb.Application.DTOs;
using QuixoWeb.Models;

namespace QuixoWeb.Controllers
{
    public class StatsController : Controller
    {
        private readonly QuixoDbContext _context;
        
        public StatsController(QuixoDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            // Obtener todos los juegos finalizados
            var allGames = await _context.Games
                .Where(g => g.EndedAt.HasValue)
                .ToListAsync();
            
            // Estadísticas modo 2 jugadores
            var twoPlayerGames = allGames.Where(g => g.Mode == 2).ToList();
            var playerStats = CalculatePlayerStats(twoPlayerGames);
            
            // Estadísticas modo 4 jugadores
            var fourPlayerGames = allGames.Where(g => g.Mode == 4).ToList();
            var teamStats = CalculateTeamStats(fourPlayerGames);
            
            ViewBag.PlayerStats = playerStats;
            ViewBag.TeamStats = teamStats;
            
            return View();
        }
        
        private List<PlayerStatsDto> CalculatePlayerStats(List<Game> games)
        {
            var stats = new List<PlayerStatsDto>();
            
            if (!games.Any())
                return stats;
            
            // Contar victorias por jugador
            var playerWins = games
                .Where(g => g.WinnerPlayerId.HasValue)
                .GroupBy(g => g.WinnerPlayerId!.Value) 
                .ToDictionary(g => g.Key, g => g.Count());
            
            // Jugador 1
            stats.Add(new PlayerStatsDto
            {
                PlayerId = 1,
                Name = "Jugador 1 (O)",
                GamesPlayed = games.Count,
                GamesWon = playerWins.ContainsKey(1) ? playerWins[1] : 0
            });
            
            // Jugador 2
            stats.Add(new PlayerStatsDto
            {
                PlayerId = 2,
                Name = "Jugador 2 (X)",
                GamesPlayed = games.Count,
                GamesWon = playerWins.ContainsKey(2) ? playerWins[2] : 0
            });
            
            return stats;
        }

        private List<TeamStatsDto> CalculateTeamStats(List<Game> games)
        {
            var stats = new List<TeamStatsDto>();
            
            if (!games.Any())
                return stats;
            
            // Contar victorias por equipo
            var teamWins = games
                .Where(g => g.WinnerTeamId.HasValue)
                .GroupBy(g => g.WinnerTeamId!.Value) 
                .ToDictionary(g => g.Key, g => g.Count());
            
            // Equipo A
            stats.Add(new TeamStatsDto
            {
                TeamId = 1,
                Name = "Equipo A",
                GamesPlayed = games.Count,
                GamesWon = teamWins.ContainsKey(1) ? teamWins[1] : 0
            });
            
            // Equipo B
            stats.Add(new TeamStatsDto
            {
                TeamId = 2,
                Name = "Equipo B",
                GamesPlayed = games.Count,
                GamesWon = teamWins.ContainsKey(2) ? teamWins[2] : 0
            });
            
            return stats;
        }
    }
}