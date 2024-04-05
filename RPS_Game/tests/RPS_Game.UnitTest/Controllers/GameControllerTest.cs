using Microsoft.AspNetCore.Mvc;
using Moq;
using RPS_Game.API.Controllers;
using RPS_Game.API.Entities;
using RPS_Game.API.Models;
using RPS_Game.API.Repository;
using RPS_Game.API.Utilities;

namespace RPS_Game.UnitTest.Controllers
{
    [TestClass]
    public class GameControllerTest
    {
        private Mock<IGameRepository> _gameRepositoryMock = null!;
        private GameController _gameController = null!;

        [TestInitialize]
        public void Initialize()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
            _gameController = new GameController(_gameRepositoryMock.Object);

        }



        [TestCleanup]
        public void Cleanup()
        {

        }

        [TestMethod]
        public async Task GetGameInfo_ReturnOk()
        {
            //Arrange
            int gameId = 1;
            Game NewGame = new Game();
            _gameRepositoryMock.Setup(x => x.GetGameByIdAsync(gameId)).ReturnsAsync(NewGame);

            //Act 
            var result = await _gameController.GetGameInfo(gameId);
            var okResponse = result as OkObjectResult;

            //Assert
            Assert.IsNotNull(okResponse);
            var response = okResponse.Value as Result<Game>;
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccess);
            Assert.IsInstanceOfType(response.Value, typeof(Game));
        }

        [TestMethod]
        public async Task GetGameInfo_ReturnNotFound()
        {
            //Arrange
            int gameId = 1;

            //Act 
            var result = await _gameController.GetGameInfo(gameId);
            var notFoundResponse = result as NotFoundObjectResult;

            //Assert
            Assert.IsNotNull(notFoundResponse);
            var response = notFoundResponse.Value as Result;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public async Task PlayAgain_ReturnsOk()
        {
            //Arrange
            int gameId = 1;
            Game NewGame = new Game();
            Result result = Result.Success(NewGame);
            _gameRepositoryMock.Setup(x => x.GetGameByIdAsync(gameId)).ReturnsAsync(NewGame);
            _gameRepositoryMock.Setup(x => x.UpdateGameAsync(NewGame)).ReturnsAsync(result);

            //Act
            var controllerResponse = await _gameController.PlayAgain(gameId);
            var response = controllerResponse as OkObjectResult;

            //Assert
            Assert.IsNotNull(response);
            var res = response.Value as Result;
            Assert.IsNotNull(res);
            Assert.IsTrue(res.IsSuccess);

        }

        [TestMethod]
        public async Task PlayAgain_ReturnsNotFound()
        {
            //Arrange
            int gameId = 1;

            //Act 
            var result = await _gameController.PlayAgain(gameId);
            var notFoundResponse = result as NotFoundObjectResult;

            //Assert
            Assert.IsNotNull(notFoundResponse);
            var response = notFoundResponse.Value as Result;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public async Task PlayAgain_ErrorUpdating_ReturnsBadRequest()
        {
            //Arrange
            int gameId = 1;
            Game NewGame = new Game();
            Result result = Result.Failure(GameErrors.ErrorUpdatingGame);
            _gameRepositoryMock.Setup(x => x.GetGameByIdAsync(gameId)).ReturnsAsync(NewGame);
            _gameRepositoryMock.Setup(x => x.UpdateGameAsync(NewGame)).ReturnsAsync(result);

            //Act
            var controllerResponse = await _gameController.PlayAgain(gameId);
            var response = controllerResponse as BadRequestObjectResult;

            //Assert
            Assert.IsNotNull(response);
            var res = response.Value as Result;
            Assert.IsNotNull(res);
            Assert.IsFalse(res.IsSuccess);
            Assert.IsInstanceOfType(res.Error, typeof(Error));
        }

        [TestMethod]
        public async Task RegisterPlayers_ReturnsOk()
        {
            //Arrange
            RegisterPlayersQuery registerPlayersQuery = new()
            {
                player1 = Guid.NewGuid().ToString(),
                player2 = Guid.NewGuid().ToString()
            };
            Result result = Result.Success();
            _gameRepositoryMock.Setup(x => x.RegisterPlayersAsync(It.IsAny<Game>()))
                   .ReturnsAsync(result)
                   .Verifiable();

            //Act
            var controllerResponse = await _gameController.RegisterPlayers(registerPlayersQuery);
            var response = controllerResponse as OkObjectResult;

            //Assert
            Assert.IsNotNull(response);
            var res = response.Value as Result;
            Assert.IsNotNull(res);
            Assert.IsTrue(res.IsSuccess);
        }

        [TestMethod]
        public async Task PlayAgain_ErrorRegisterPlayers_ReturnsBadRequest()
        {
            //Arrange
            RegisterPlayersQuery registerPlayersQuery = new()
            {
                player1 = Guid.NewGuid().ToString(),
                player2 = Guid.NewGuid().ToString()
            };
            Result result = Result.Failure(GameErrors.ErrorCreatingNewGame);
            _gameRepositoryMock.Setup(x => x.RegisterPlayersAsync(It.IsAny<Game>())).ReturnsAsync(result);

            //Act
            var controllerResponse = await _gameController.RegisterPlayers(registerPlayersQuery);
            var response = controllerResponse as BadRequestObjectResult;

            //Assert
            Assert.IsNotNull(response);
            var res = response.Value as Result;
            Assert.IsNotNull(res);
            Assert.IsFalse(res.IsSuccess);
        }

        [TestMethod]
        public async Task RegisterMovement_ReturnsOk()
        {
            //Arrange
            RegisterMovementQuery query = new()
            {
                Player = Guid.NewGuid(),
                Movement = API.Utilities.Enums.Movement.Paper
            };

            Game game = new Game()
            {
                Player1 = query.Player,
                Player2 = Guid.NewGuid(),
            };
            Round round = new Round()
            {
                Game = game,
                RoundNumber = 1,
                Current_turn = query.Player
            };

            Result result = Result.Success();

            _gameRepositoryMock.Setup(x => x.GetGameByUserIdAsync(query.Player)).ReturnsAsync(game);
            _gameRepositoryMock.Setup(x => x.GetRoundByGameAsync(game.Id, game.Player1, game.RoundsNumber))
                .ReturnsAsync(round);
            _gameRepositoryMock.Setup(x => x.RegisterMovementAsync(round))
                .ReturnsAsync(result);
            //Act
            var controllerResponse = await _gameController.RegisterMovement(query);
            var response = controllerResponse as OkObjectResult;

            //Assert
            Assert.IsNotNull(response);
            var res = response.Value as Result<newMovementResponse>;
            Assert.IsNotNull(res);
            Assert.IsTrue(res.IsSuccess);
            Assert.IsInstanceOfType<newMovementResponse>(res.Value);

        }

        [TestMethod]
        public async Task RegisterLastMovement_ReturnsOk()
        {
            //Arrange
            RegisterMovementQuery query = new()
            {
                Player = Guid.NewGuid(),
                Movement = API.Utilities.Enums.Movement.Paper
            };

            Game game = new Game()
            {
                Player1 = Guid.NewGuid(),
                Player2 = query.Player,
                Rounds = new List<Round>()
                {
                    new Round(){ result = 1},
                    new Round(){ result = 1},
                    new Round(){ result = 1},
                }
            };
            Round round = new Round()
            {
                movement_player1 = API.Utilities.Enums.Movement.Scissors,
                Game = game,
                RoundNumber = 3,
                Current_turn = query.Player
            };

            Result result = Result.Success();

            _gameRepositoryMock.Setup(x => x.GetGameByUserIdAsync(query.Player)).ReturnsAsync(game);
            _gameRepositoryMock.Setup(x => x.GetRoundByGameAsync(game.Id, game.Player1, game.RoundsNumber))
                .ReturnsAsync(round);
            _gameRepositoryMock.Setup(x => x.RegisterMovementAsync(It.IsAny<Round>()))
                .ReturnsAsync(result).Verifiable();
            _gameRepositoryMock.Setup(x => x.UpdateGameAsync(game)).ReturnsAsync(result);

            //Act
            var controllerResponse = await _gameController.RegisterMovement(query);
            var response = controllerResponse as OkObjectResult;

            //Assert
            Assert.IsNotNull(response);
            var res = response.Value as Result<newMovementResponse>;
            Assert.IsNotNull(res);
            Assert.IsTrue(res.IsSuccess);
            Assert.IsInstanceOfType<newMovementResponse>(res.Value);

        }

        [TestMethod]
        public async Task RegisterMovement_GameNotFound_ReturnsNotFound()
        {
            //Arrange
            RegisterMovementQuery query = new()
            {
                Player = Guid.NewGuid(),
                Movement = API.Utilities.Enums.Movement.Paper
            };

            //Act
            var controllerResponse = await _gameController.RegisterMovement(query);
            var notFoundResponse = controllerResponse as NotFoundObjectResult;

            //Assert
            Assert.IsNotNull(notFoundResponse);
            var response = notFoundResponse.Value as Result;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
            Assert.IsInstanceOfType<Error>(response.Error);

        }

        [TestMethod]
        public async Task RegisterMovement_ErrorExpectedRounds_ReturnsBadRequest()
        {
            //Arrange
            RegisterMovementQuery query = new()
            {
                Player = Guid.NewGuid(),
                Movement = API.Utilities.Enums.Movement.Paper
            };

            Game game = new Game()
            {
                Player1 = Guid.NewGuid(),
                Player2 = query.Player,
            };


            Result result = Result.Success();

            _gameRepositoryMock.Setup(x => x.GetGameByUserIdAsync(query.Player)).ReturnsAsync(game);
            _gameRepositoryMock.Setup(x => x.GetRoundByGameAsync(game.Id, game.Player1, game.RoundsNumber));


            //Act
            var controllerResponse = await _gameController.RegisterMovement(query);
            var response = controllerResponse as BadRequestObjectResult;

            //Assert
            Assert.IsNotNull(response);
            var res = response.Value as Result;
            Assert.IsNotNull(res);
            Assert.IsFalse(res.IsSuccess);
            Assert.IsInstanceOfType<Error>(res.Error);

        }

        [TestMethod]
        public async Task RegisterMovement_ItsNotYourTurn_ReturnsBadRequest()
        {
            //Arrange
            RegisterMovementQuery query = new()
            {
                Player = Guid.NewGuid(),
                Movement = API.Utilities.Enums.Movement.Paper
            };

            Game game = new Game()
            {
                Player1 = Guid.NewGuid(),
                Player2 = query.Player,
            };


            Result result = Result.Success();

            Round round = new Round()
            {
                movement_player1 = API.Utilities.Enums.Movement.Scissors,
                Game = game,
                RoundNumber = 3,
                Current_turn = game.Player1
            };

            _gameRepositoryMock.Setup(x => x.GetGameByUserIdAsync(query.Player)).ReturnsAsync(game);
            _gameRepositoryMock.Setup(x => x.GetRoundByGameAsync(game.Id, game.Player1, game.RoundsNumber))
               .ReturnsAsync(round);

            //Act
            var controllerResponse = await _gameController.RegisterMovement(query);
            var response = controllerResponse as BadRequestObjectResult;

            //Assert
            Assert.IsNotNull(response);
            var res = response.Value as Result;
            Assert.IsNotNull(res);
            Assert.IsFalse(res.IsSuccess);
            Assert.IsInstanceOfType<Error>(res.Error);

        }

        [TestMethod]
        public async Task RegisterMovement_ErrorRegisterMovement_ReturnsBadRequest()
        {
            //Arrange
            RegisterMovementQuery query = new()
            {
                Player = Guid.NewGuid(),
                Movement = API.Utilities.Enums.Movement.Paper
            };

            Game game = new Game()
            {
                Player1 = Guid.NewGuid(),
                Player2 = query.Player,
            };


            Result result = Result.Failure(GameErrors.ErrorUpdatingRound);

            Round round = new Round()
            {
                movement_player1 = API.Utilities.Enums.Movement.Scissors,
                Game = game,
                RoundNumber = 3,
                Current_turn = game.Player2
            };

            _gameRepositoryMock.Setup(x => x.GetGameByUserIdAsync(query.Player)).ReturnsAsync(game);
            _gameRepositoryMock.Setup(x => x.GetRoundByGameAsync(game.Id, game.Player1, game.RoundsNumber))
               .ReturnsAsync(round);
            _gameRepositoryMock.Setup(x => x.RegisterMovementAsync(It.IsAny<Round>()))
                .ReturnsAsync(result).Verifiable();

            //Act
            var controllerResponse = await _gameController.RegisterMovement(query);
            var response = controllerResponse as BadRequestObjectResult;

            //Assert
            Assert.IsNotNull(response);
            var res = response.Value as Result;
            Assert.IsNotNull(res);
            Assert.IsFalse(res.IsSuccess);
            Assert.IsInstanceOfType<Error>(res.Error);

        }

        [TestMethod]
        public async Task RegisterMovement_ErrorUpdatingGame_ReturnsBadRequest()
        {
            //Arrange
            RegisterMovementQuery query = new()
            {
                Player = Guid.NewGuid(),
                Movement = API.Utilities.Enums.Movement.Paper
            };

            Game game = new Game()
            {
                Player1 = Guid.NewGuid(),
                Player2 = query.Player,
                Rounds = new List<Round>()
                {
                    new Round(){ result = 1},
                    new Round(){ result = 1},
                    new Round(){ result = 1},
                }
            };
            Round round = new Round()
            {
                movement_player1 = API.Utilities.Enums.Movement.Scissors,
                Game = game,
                RoundNumber = 3,
                Current_turn = query.Player
            };

            Result result = Result.Success();
            Result resultFail = Result.Failure(GameErrors.ErrorUpdatingGame);

            _gameRepositoryMock.Setup(x => x.GetGameByUserIdAsync(query.Player)).ReturnsAsync(game);
            _gameRepositoryMock.Setup(x => x.GetRoundByGameAsync(game.Id, game.Player1, game.RoundsNumber))
               .ReturnsAsync(round);
            _gameRepositoryMock.Setup(x => x.RegisterMovementAsync(It.IsAny<Round>()))
                .ReturnsAsync(result).Verifiable();
            _gameRepositoryMock.Setup(x => x.UpdateGameAsync(game)).ReturnsAsync(resultFail);

            //Act
            var controllerResponse = await _gameController.RegisterMovement(query);
            var response = controllerResponse as BadRequestObjectResult;

            //Assert
            Assert.IsNotNull(response);
            var res = response.Value as Result;
            Assert.IsNotNull(res);
            Assert.IsFalse(res.IsSuccess);
            Assert.IsInstanceOfType<Error>(res.Error);
        }


        //[TestMethod]
        //public async Task PlayAgain_ErrorUpdating_ReturnsBadRequest()
        //{
        //    //Arrange
        //    //Act
        //    //Assert
        //}
    }
}
