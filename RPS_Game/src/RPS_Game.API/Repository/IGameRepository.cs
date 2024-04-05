using RPS_Game.API.Entities;
using RPS_Game.API.Utilities;
using RPS_Game.API.Utilities.Enums;

namespace RPS_Game.API.Repository
{
    public interface IGameRepository
    {
        Task<Game?> GetGameByIdAsync(int id);
        Task<Game?> GetGameByUserIdAsync(Guid id);

        Task<Result> RegisterPlayersAsync(Game newGame);

        Task<Result> RegisterMovementAsync(Round round);

        Task<Round?> GetRoundByGameAsync(int GameID, Guid Player, int roundNumber);

        Task<Result> UpdateGameAsync(Game game);
    }
}
