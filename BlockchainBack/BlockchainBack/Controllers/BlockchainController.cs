using BlockchainBack.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainBack.Controllers
{
    [ApiController]
    [System.Web.Http.Cors.EnableCors(origins: "*", headers: "*", methods: "*")]
    [Route("[controller]")]
    public class BlockchainController : ControllerBase
    {
        [HttpGet(Name = "GetBlockchain")]
        public Blockchain Get()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("");
            Console.WriteLine($"Notificacion de la API: GetBlockchain() ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" Obteniendo la Blockchain.");

            var blockChain = Program.GetBlockchain();
            blockChain.Wait();
            Console.ResetColor();

            return blockChain.Result;
        }

        [HttpPost("mine", Name = "Mine")]
        public IActionResult Mine()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("");
            Console.WriteLine($"Notificacion de la API: Mine() ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" Minando un bloque.");

            var output = Program._blockchainServices.MineBlock();
            output.Wait();


            Console.ResetColor();

            return Ok();
        }

        [HttpDelete]
        public void DeleteBlockchain()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("");
            Console.WriteLine($"Notificacion de la API: DeleteBlockchain ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" Eliminando la Blockchain");

            new MongoDbRepository().DeleteAll();
            Console.ResetColor();
        }
    }
}