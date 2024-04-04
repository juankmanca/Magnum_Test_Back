using RPS_Game.API.Utilities.Enums;
using System.ComponentModel.DataAnnotations;

namespace RPS_Game.API.Models
{
    public class RegisterMovementQuery
    {

        //[Required]
        //[Range(1, 3, ErrorMessage = "The Round number must be between {0} and {1}")]
        //public int Round {  get; set; }

        [Required]
        public Guid Player { get; set; }

        [Required]
        [MovementValidation(ErrorMessage = "Invalid movement")]
        public Movement Movement { get; set; }
    }
}
