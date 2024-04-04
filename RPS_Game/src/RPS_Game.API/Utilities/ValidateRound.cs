using RPS_Game.API.Entities;
using RPS_Game.API.Models;
using RPS_Game.API.Utilities.Enums;

namespace RPS_Game.API.Utilities
{
    public class ValidateRound
    {
        // Definir una matriz para almacenar los resultados de los enfrentamientos
        // El valor en la posición [i, j] representa el resultado cuando el jugador 1 hace el movimiento i y el jugador 2 hace el movimiento j.
        private static int[,] results = {
            {0, 2, 1},  // Piedra vs. Piedra, Piedra vs. Papel, Piedra vs. Tijeras
            {1, 0, 2},  // Papel vs. Piedra, Papel vs. Papel, Papel vs. Tijeras
            {2, 1, 0}   // Tijeras vs. Piedra, Tijeras vs. Papel, Tijeras vs. Tijeras
        };


        public static int ValidateMovements(Movement player1, Movement player2)
        {
            // Obtener los índices correspondientes a los movimientos de los jugadores
            int index1 = (int)player1;
            int index2 = (int)player2;

            // Devolver el resultado del enfrentamiento según la matriz
            return results[index1 - 1, index2 - 1];
        }

        public static Result Validate(Guid Player, Movement movement, Round round)
        {
            if (round.Current_turn != Player) return Result.Failure(GameErrors.ItsNotYourTurn);

            if (round.movement_player1 == Movement.None)
            {
                round.movement_player1 = movement;
                round.Current_turn = round.Game!.Player2;
            }
            else
            {
                round.movement_player2 = movement;
                round.Current_turn = null;
            }

            if (round.movement_player1 != Movement.None && round.movement_player2 != Movement.None)
            {
                round.result = ValidateMovements(round.movement_player1, round.movement_player2);
            }

            return Result.Success(round);
        }

        public static int ValidateWinner(ICollection<Round> rounds)
        {
            int scorePlayer1 = 0;
            int scorePlayer2 = 0;

            foreach (var round in rounds)
            {
                if (round.result == 1) scorePlayer1++;
                if (round.result == 2) scorePlayer1++;
            }

            if (scorePlayer1 == scorePlayer2) return 0;

            if (scorePlayer1 > scorePlayer2)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
    }
}
