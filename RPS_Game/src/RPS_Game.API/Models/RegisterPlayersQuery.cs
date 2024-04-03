using System.ComponentModel.DataAnnotations;

namespace RPS_Game.API.Models
{
    public sealed class RegisterPlayersQuery
    {
        [Required]
        [MaxLength(50)]
        public string player1 {  get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string player2 { get; set; } = string.Empty;
    }
}
