namespace RPS_Game.API.Utilities
{
    public record Error(string Code, string Name)
    {
        public static Error None = new Error(string.Empty, string.Empty);
        public static Error NullValue = new("Error.NullValue", "Un valor null fue ingresado");
    }
}
