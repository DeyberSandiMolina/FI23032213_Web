using Microsoft.AspNetCore.Mvc;
using QuixoWeb.Application.Services;
using QuixoWeb.Application.DTOs;

namespace QuixoWeb.Controllers
{
    public class GameController : Controller
    {
        private readonly GameService _gameService;

        public GameController(GameService gameService)
        {
            _gameService = gameService;
        }

        // GET: /Game/Create?mode4players=true
        [HttpGet]
        public async Task<IActionResult> Create(bool mode4players = false)
        {
            Console.WriteLine($"=== GameController.Create ===");
            Console.WriteLine($"Parámetro mode4players: {mode4players}");
            
            int gameId = await _gameService.CreateGameAsync(mode4players);
            
            Console.WriteLine($"Juego creado con ID: {gameId}");
            Console.WriteLine($"Redirigiendo a /Game/State?gameId={gameId}");
            
            return RedirectToAction("State", new { gameId });
        }

        // GET: /Game/State?gameId=1
        [HttpGet]
        public async Task<IActionResult> State(int gameId)
        {
            Console.WriteLine($"=== GameController.State ===");
            Console.WriteLine($"gameId: {gameId}");
            
            try
            {
                var state = await _gameService.GetStateAsync(gameId);

                ViewBag.GameId = gameId;
                ViewBag.IsFourPlayerMode = state.IsFourPlayerMode;
                
                Console.WriteLine($"Estado obtenido - Modo 4 jugadores: {state.IsFourPlayerMode}");
                Console.WriteLine($"Turno actual: {state.CurrentPlayer}, Símbolo: {state.CurrentSymbol}");

                return View(state);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en GameController.State: {ex.Message}");
                return RedirectToAction("Create", new { mode4players = false });
            }
        }

        // POST: /Game/Move
        [HttpPost]
        public async Task<IActionResult> Move(int gameId, [FromBody] MoveRequestDto move)
        {
            Console.WriteLine($"=== GameController.Move ===");
            Console.WriteLine($"gameId: {gameId}");
            Console.WriteLine($"Movimiento: ({move.TakeRow},{move.TakeCol}) -> ({move.PlaceRow},{move.PlaceCol})");
            Console.WriteLine($"Orientation: {move.Orientation}");
            
            var result = await _gameService.MakeMoveAsync(gameId, move);
            
            Console.WriteLine($"Resultado: Success={result.Success}, Message={result.Message}");
            Console.WriteLine($"Nuevo jugador: {result.CurrentPlayer}, Símbolo: {result.CurrentSymbol}");
            
            return Json(new {
                success = result.Success,
                message = result.Message,
                board = result.Board,
                currentPlayer = result.CurrentPlayer,
                currentSymbol = result.CurrentSymbol,
                isGameOver = result.IsGameOver,
                winnerPlayer = result.WinnerPlayer
            });
        }
    }
}