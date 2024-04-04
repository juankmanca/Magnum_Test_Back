using RPS_Game.API.Utilities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPS_Game.API.Entities
{
    public class Round
    {
        public int Id { get; set; }

        [ForeignKey("GameId")]
        public int GameId { get; set; }
        public Game? Game { get; set; }
        public Movement movement_player1 { get; set; }
        public Movement movement_player2 { get; set; }
        public int result { get; set; }
        public int RoundNumber { get; set; }
        public Guid? Current_turn { get; set; }


        public Round()
        {
            
        }

        private Round(int id, int gameId, Movement movement_player1, Movement movement_player2, int result, Guid current_turn, int roundNumber)
        {
            Id = id;
            GameId = gameId;
            this.movement_player1 = movement_player1;
            this.movement_player2 = movement_player2;
            this.result = result;
            Current_turn = current_turn;
            RoundNumber = roundNumber;
        }

        public static Round Crate(int gameId, Guid current_turn, int round)
        {
            return new Round(
                    0,
                    gameId,
                    0,
                    0,
                    0,
                    current_turn,
                    round
                );
        }

    }
}
