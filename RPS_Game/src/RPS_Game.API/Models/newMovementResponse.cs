using RPS_Game.API.Entities;

namespace RPS_Game.API.Models
{
    public class newMovementResponse
    {
        public bool finishGame { get; set; } = false;

        public int result { get; set; } = 0;

        public Round? round { get; set; }

    }
}
