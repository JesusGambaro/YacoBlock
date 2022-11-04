using BlockchainBack.Models;
using BlockchainBack.Services;

namespace BlockchainBack;

public static class Program
{
    private static string _baseAddress = null!;
    public static BlockchainServices _blockchainServices = null!;
    private static NodeServices _nodeServices = null!;
    public static MongoDbRepository mongoDbRepository = new();

    private static void Main(string[] args)
    {
        _baseAddress = args.Length > 0 ? args[0] : "http://localhost:9000/";
        if (!_baseAddress.EndsWith("/")) _baseAddress += "/";

        //get blockchain from database 
        var blockchain = mongoDbRepository.GetBlockchain();
        blockchain.Wait();
        Console.WriteLine("Blockchain loaded" + blockchain.Result.Id);
        _blockchainServices = new BlockchainServices(blockchain.Result);
        _nodeServices = new NodeServices(blockchain.Result);

        new Startup().CreateHost(args);
    }

    public static async Task<Blockchain> GetBlockchain()
    {
        await mongoDbRepository.UpdateBlockchain(_blockchainServices.Blockchain);
        return _blockchainServices.Blockchain;
    }
}