using System.Reflection;
using BlockchainBack.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace BlockchainBack.Controllers
{
    [ApiController]
    [System.Web.Http.Cors.EnableCors(origins: "*", headers: "*", methods: "*")]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        [HttpPost]
        [Route("AddTransaction"), Produces("application/json")]
        public IActionResult AddTransaction([FromBody] Transaction newTransaction)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("");
            Console.WriteLine($"Notificacion de la API: Transactions-add {newTransaction.Id} from {newTransaction.Sender.Address} to {newTransaction.Receiver.Address} \n with {newTransaction.Amount} pesos");
            Console.ForegroundColor = ConsoleColor.Cyan;
            var blockchain = Program.GetBlockchain();
            blockchain.Wait();
            var foundTransaction = blockchain.Result.PendingTransactions
                .FirstOrDefault(pt => pt.Id == newTransaction.Id);

            if (foundTransaction == null)
            {
                Console.WriteLine(
                    $"Transaccion: {newTransaction.Amount} desde {newTransaction.Sender.Address} a {newTransaction.Receiver.Address}.");
                Program._blockchainServices.AddTransaction(newTransaction);
                /*if (!sender) return;
                Program._blockchainServices(newTransaction);*/
            }
            else
            {
                Console.WriteLine($"La transaccion ya existe en el nodo.");
                return BadRequest("La transaccion ya existe en el nodo.");
            }

            Console.ResetColor();
            return Ok(Program._blockchainServices.PendingTransactions().Last());
        }

        [HttpGet]
        [Route("GetPendingTransactions"), Produces("application/json")]
        public IActionResult GetPendingTransactions()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("");
            Console.WriteLine("Notificacion de la API: Transactions-get");
            Console.ForegroundColor = ConsoleColor.Cyan;

            var pendingTransactions = Program._blockchainServices.PendingTransactions();
            Console.WriteLine($"Se han encontrado {pendingTransactions.Count} transacciones pendientes.");
            Console.ResetColor();
            return Ok(pendingTransactions);
        }

        //dELETE TRANSACTION
        [HttpDelete]
        [Route("DeleteTransaction"), Produces("application/json")]
        public IActionResult DeleteTransaction(string id)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("");
            Console.WriteLine("Notificacion de la API: Transactions-delete");
            Console.ForegroundColor = ConsoleColor.Cyan;
            var pendingTransactions = Program._blockchainServices.PendingTransactions();
            if (pendingTransactions.All(tr => tr.Id != id))
            {
                Console.WriteLine("No se ha encontrado la transaccion.");
                Console.ResetColor();
                return NotFound();
            }

            Program._blockchainServices.DeleteTransaction(id);

            Console.WriteLine("Transaccion eliminada");
            Console.ResetColor();
            return Ok(pendingTransactions);
        }

        [HttpGet]
        [Route("GetLibrosMayores"), Produces("application/json")]
        public IActionResult GetTransactions()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("");
            Console.WriteLine("Notificacion de la API: Transactions-get");
            Console.ForegroundColor = ConsoleColor.Cyan;

            var librosMayores = Program.GetLibrosMayores(Program._blockchainServices);
            librosMayores.Wait();
            Console.WriteLine($"Se han encontrado {librosMayores.Result.Count} transacciones.");
            Console.ResetColor();
            return Ok(librosMayores.Result);
        }
    }
}