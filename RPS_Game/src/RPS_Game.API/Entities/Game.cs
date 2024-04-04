namespace RPS_Game.API.Entities
{
    public class Game
    {
       

        public int Id { get; set; }
        public Guid Player1 { get; set; }
        public string Player1Name { get; set; } = string.Empty;

        public Guid Player2 { get; set; }
        public string Player2Name { get; set; } = string.Empty;

        public int Result { get; set; }

        public ICollection<Round>? Rounds { get; set; }
        public int RoundsNumber => Rounds == null ? 0 : Rounds.Count;

        public Game()
        {

        }

        private Game(int id, Guid player1, string player1Name, Guid player2, string player2Name, int result)
        {
            Id = id;
            Player1 = player1;
            Player1Name = player1Name;
            Player2 = player2;
            Player2Name = player2Name;
            Result = result;
        }

        public static Game Craate(string Player1, string Player2)
        {
            return new(
                    0,
                    Guid.NewGuid(),
                    Player1,
                    Guid.NewGuid(),
                    Player2,
                    0
                );
        }
    }
}
