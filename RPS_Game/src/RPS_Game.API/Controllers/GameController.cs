using Microsoft.AspNetCore.Mvc;
using RPS_Game.API.Models;

namespace RPS_Game.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class GameController : ControllerBase
    {
        [HttpGet]
        [ActionName("hi")]
        public async Task<IActionResult> Hi()
        {

            return Ok();
        }


        [HttpPost]
        [ActionName("registerPlayers")]

        public async Task<IActionResult> RegisterPlayers([FromBody] RegisterPlayersQuery request)
        {
            Console.Write(request.player1);

            return Ok();
        }
    }
}
