using RPS_Game.API.Utilities;

namespace RPS_Game.API.Models
{
    public static class GameErrors
    {
        public static Error ErrorCreatingNewGame = new("Game.ErrorCreatingNewGame", "Internal Error creating new Game");
        public static Error ErrorUpdatingRound = new("Game.ErrorUpdatingRound", "Internal Error updating round");
        public static Error ErrorUpdatingGame = new("Game.ErrorUpdatingGame", "Internal Error updating game");
        public static Error NotFound = new("Game.NotFound", "Game Not found");
        public static Error ItsNotYourTurn = new("Game.ItsNotYourTurn", "It's not your turn, please wait for your opponent's turn.");
        public static Error ExcedeExpectedRounds = new("Game.ExcedeExpectedRounds", "This game already finish.");
    }
}
