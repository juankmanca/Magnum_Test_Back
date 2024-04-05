using Microsoft.EntityFrameworkCore;
using RPS_Game.API.Data;
using RPS_Game.API.Entities;
using RPS_Game.API.Models;
using RPS_Game.API.Utilities;

namespace RPS_Game.API.Repository
{
    public class GameRepository : IGameRepository
    {
        private readonly ApplicationDbContext _context;

        public GameRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Game?> GetGameByIdAsync(int id)
        {
            return await _context.Games
                .Include(g => g.Rounds)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Result> UpdateGame(Game game)
        {
            try
            {
                _context.Games.Update(game);

                await _context.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(GameErrors.ErrorUpdatingGame);
            }
        }

        public async Task<Game?> GetGameByUserIdAsync(Guid id)
        {
            return await _context.Games
                .Include(g => g.Rounds)
                .FirstOrDefaultAsync(x => x.Player1 == id || x.Player2 == id);
        }

        public async Task<Result> RegisterMovementAsync(Round round)
        {
            try
            {
                _context.Rounds.Update(round);
                await _context.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(GameErrors.ErrorUpdatingRound);
            }
        }

        public async Task<Round?> GetRoundByGameAsync(int GameID, Guid Player, int roundNumber)
        {
            Round? round = await _context.Rounds.FirstOrDefaultAsync(x => x.RoundNumber == roundNumber && x.GameId == GameID);

            if (round is not null && round.Current_turn != null) return round;

            int newRoundNumber = roundNumber + 1;

            if (newRoundNumber > 3) return null;

            var newRound = Round.Crate(GameID, Player, newRoundNumber);

            _context.Rounds.Add(newRound);

            await _context.SaveChangesAsync();

            return newRound;
        }


        public async Task<Result> RegisterPlayersAsync(Game newGame)
        {
            try
            {
                _context.Games.Add(newGame);
                await _context.SaveChangesAsync();

                return Result.Success(newGame.Id);
            }
            catch (Exception ex)
            {
                return Result.Failure(GameErrors.ErrorCreatingNewGame);
            }
        }
    }
}
