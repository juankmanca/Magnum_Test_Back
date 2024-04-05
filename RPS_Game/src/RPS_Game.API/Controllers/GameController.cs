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

        [HttpGet]
        [ActionName("getGameInfo/{id}")]
        public async Task<IActionResult> GetGameInfo([FromRoute] int id)
        {
            Game? game = await _gameRepository.GetGameByIdAsync(id);
            if(game is null) return NotFound(Result.Failure(GameErrors.NotFound));

            var response = Result.Success(game);

            return Ok(response);
        }

        [HttpGet]
        [ActionName("playAgain/{id}")]

        public async Task<IActionResult> playAgain([FromRoute] int id)
        {
            Game? game = await _gameRepository.GetGameByIdAsync(id);
            if (game is null) return NotFound(Result.Failure(GameErrors.NotFound));

            game.Result = 0;
            game.Rounds = null;

            var result = await _gameRepository.UpdateGame(game);

            if (result.IsFailure) return BadRequest(result);

            return Ok(result);
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
            Game? game = await _gameRepository.GetGameByUserIdAsync(query.Player);
            if (game is null) return NotFound(Result.Failure(GameErrors.NotFound));

            var result = Result.Create(new newMovementResponse());

            Round? round = await _gameRepository.GetRoundByGameAsync(game.Id, game.Player1, game.RoundsNumber);

            var ValidatedRound = ValidateRound.Validate(query.Player, query.Movement, round);

            if (ValidatedRound is null) return BadRequest(Result.Failure(GameErrors.ItsNotYourTurn));

            var dbResponse = await _gameRepository.RegisterMovementAsync(ValidatedRound);

            if (dbResponse.IsFailure) return BadRequest(dbResponse);

            if(ValidatedRound.Current_turn == null && ValidatedRound.RoundNumber == 3)
            {
                //Calculate winner
                int WinnerPlayer = ValidateRound.ValidateWinner(game.Rounds!);

                game.Result = WinnerPlayer;

                var updateResult = await _gameRepository.UpdateGame(game);

                if (updateResult.IsFailure) return BadRequest(updateResult);

                result.Value.finishGame = true;
                result.Value.result = WinnerPlayer;
            }


            result.Value.round = ValidatedRound;

            return Ok(result);

        }
    }
}
