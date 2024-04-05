using Microsoft.EntityFrameworkCore;
using Moq;
using RPS_Game.API.Controllers;
using RPS_Game.API.Data;
using RPS_Game.API.Entities;
using RPS_Game.API.Models;
using RPS_Game.API.Repository;
using RPS_Game.API.Utilities;
using RPS_Game.UnitTest.Shared;

namespace RPS_Game.UnitTest.Services
{
    [TestClass]
    public class GameRepositoryTest
    {
        private ApplicationDbContext _context = null!;
        private GameRepository _gameRepository = null!;
        private GameRepository _gameRepositoryException = null!;
        private ExceptionalDataContext _exceptionContext = null!;
        private Guid player = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            var _options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options;

            _exceptionContext = new ExceptionalDataContext(_options);
            _gameRepositoryException = new GameRepository(_exceptionContext);
            _context = new ApplicationDbContext(_options);
            _gameRepository = new GameRepository(_context);
        }


        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        private async Task AddGameAsync()
        {
            Game game = new Game()
            {
                Player1 = player,
                Player2 = Guid.NewGuid(),
                Rounds = new List<Round>()
                {
                    new Round() { Current_turn = player, RoundNumber = 1 }
                }
            };

            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();
        }

        [TestMethod]
        public async Task GetGameByIdAsync_ReturnsGame()
        {
            //Arrange
            int gameId = 1;
            await AddGameAsync();

            //Act
            Game? newGame = await _gameRepository.GetGameByIdAsync(gameId);

            //Assert
            Assert.IsNotNull(newGame);
            Assert.AreEqual(newGame.Id, 1);
        }

        [TestMethod]
        public async Task GetGameByIdAsync_ReturnsNull()
        {
            // Arrange: No se agrega ningún juego en la base de datos

            // Act
            Game? nonExistentGame = await _gameRepository.GetGameByIdAsync(999);

            // Assert
            Assert.IsNull(nonExistentGame);
        }

        [TestMethod]
        public async Task UpdateGameAsync_ReturnsGameUpdated()
        {
            //Arrange
            await AddGameAsync();
            Game? game = await _context.Games.FirstOrDefaultAsync(x => x.Id == 1);
            Assert.IsNotNull(game);

            game.Result = 1;

            // Act
            Result result = await _gameRepository.UpdateGameAsync(game);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task UpdateGameAsync_ReturnsUpdatedException()
        {
            //Arrange
            var game = new Game();

            // Act
            Result result = await _gameRepositoryException.UpdateGameAsync(game);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsInstanceOfType<Error>(result.Error);
        }

        [TestMethod]
        public async Task GetGameByUserIdAsync_ReturnsGame()
        {
            //Arrange
            await AddGameAsync();

            //Act
            Game? newGame = await _gameRepository.GetGameByUserIdAsync(player);

            //Assert
            Assert.IsNotNull(newGame);
            Assert.AreEqual(newGame.Id, 1);
        }

        [TestMethod]
        public async Task RegisterMovementAsync_ReturnsSuccessResult()
        {
            //Arrange
            await AddGameAsync();
            Round ? round = await _context.Rounds.FirstOrDefaultAsync(x => x.Id == 1);
            Assert.IsNotNull(round);

            //Act
            Result result = await _gameRepository.RegisterMovementAsync(round);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task RegisterMovementAsync_ReturnsFailureResult()
        {
            //Arrange
            await AddGameAsync();
            Round? round = await _context.Rounds.FirstOrDefaultAsync(x => x.Id == 1);
            Assert.IsNotNull(round);

            //Act
            Result result = await _gameRepositoryException.RegisterMovementAsync(round);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsInstanceOfType<Error>(result.Error);
        }

        [TestMethod]
        public  async Task GetRoundByGameAsync_ReturnsRound()
        {
            //Arrange
            await AddGameAsync();

            //Act
            Round? round = await _gameRepository.GetRoundByGameAsync(1, player, 1);

            //Assert
            Assert.IsNotNull(round);
        }

        [TestMethod]
        public async Task GetRoundByGameAsync_ReturnsNewRound()
        {
            //Arrange
            await AddGameAsync();

            //Act
            Round? round = await _gameRepository.GetRoundByGameAsync(1, player, 2);

            //Assert
            Assert.IsNotNull(round);
        }

        [TestMethod]
        public async Task GetRoundByGameAsync_FinishRound_ReturnsNull()
        {
            //Arrange
            await AddGameAsync();

            //Act
            Round? round = await _gameRepository.GetRoundByGameAsync(1, player, 3);

            //Assert
            Assert.IsNull(round);
        }

        [TestMethod]
        public async Task RegisterPlayersAsync_ReturnsSuccessResult()
        {
            //Arrange
            Game game = new Game();

            //Act
            var result = await _gameRepository.RegisterPlayersAsync(game);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task RegisterPlayersAsync_ReturnsFailureResult()
        {
            //Arrange
            Game game = new Game();

            //Act
            var result = await _gameRepositoryException.RegisterPlayersAsync(game);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsInstanceOfType<Error>(result.Error);
        }
    }
}
