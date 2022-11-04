using BlockchainBack.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainBack.Controllers;

[ApiController]
[System.Web.Http.Cors.EnableCors(origins: "*", headers: "*", methods: "*")]
[Route("[controller]")]
public class BlockController : Controller
{
    [HttpGet]
    [Route("GetBalance"), Produces("application/json")]
    public IActionResult GetBalance(string address)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("");
        Console.WriteLine("Notificacion de la API: GetBalance");
        Console.ForegroundColor = ConsoleColor.Cyan;
        var balance = Program._blockchainServices.Balance(address);
        Console.WriteLine($"Balance de {address} es: {balance}.");
        Console.ResetColor();
        return Ok(balance);
    }
}