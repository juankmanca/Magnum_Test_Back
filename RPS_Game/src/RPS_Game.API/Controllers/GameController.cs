using Microsoft.AspNetCore.Mvc;
using RPS_Game.API.Entities;
using RPS_Game.API.Models;
using RPS_Game.API.Repository;
using RPS_Game.API.Utilities;

namespace RPS_Game.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class GameController : ControllerBase
    {
        private readonly IGameRepository _gameRepository;

        public GameController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        [HttpPost]
        [ActionName("registerPlayers")]

        public async Task<IActionResult> RegisterPlayers([FromBody] RegisterPlayersQuery query)
        {
            Game NewGame = Game.Craate(query.player1, query.player2);

            Result result = await _gameRepository.RegisterPlayersAsync(NewGame);

            if (result.IsFailure) return BadRequest(result);

            return Ok(result);
        }

        [HttpPost]
        [ActionName("registerMovement")]

        public async Task<IActionResult> registerMovement([FromBody] RegisterMovementQuery query)
        {
            Game? game = await _gameRepository.GetGameByIdAsync(query.Player);

            if (game is null)
            {
                var result = Result.Failure(GameErrors.NotFound);
                return NotFound(result);
            }

            Round? round = await _gameRepository.GetRoundByGameAsync(game.Id, game.Player1, game.RoundsNumber);
            if(round is null)
            {
                //Calculate winner
                int WinnerPlayer = ValidateRound.ValidateWinner(game.Rounds!);

                //TODO: Register winnr

                return Ok(WinnerPlayer);
            }

            var response = ValidateRound.Validate(query.Player, query.Movement, round);

            if (response.IsFailure) return BadRequest(response);

            var dbResponse = await _gameRepository.RegisterMovementAsync(round);

            if (dbResponse.IsFailure) return BadRequest(dbResponse);

            return Ok(dbResponse);

        }
    }
}
