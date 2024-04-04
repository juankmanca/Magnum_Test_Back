using System.ComponentModel.DataAnnotations;

namespace RPS_Game.API.Utilities.Enums
{
    public enum Movement
    {
        None = 0,
        Rock = 1,
        Paper = 2,
        Scissors = 3
    }

    public class MovementValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Null values are handled by [Required] attribute
            }

            int movementValue = (int)value;
            if (!Enum.IsDefined(typeof(Movement), movementValue) || movementValue == 0)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
