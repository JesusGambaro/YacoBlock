using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace BlockchainBack.Models;

public class Blockchain
{
    public ObjectId Id { get; set; }
    public List<Block> Chain { get; set; }
    public List<string> Nodes;
    public int Difficulty;
    public List<Transaction> PendingTransactions { get; set; }

    public Blockchain()
    {
        Chain = new List<Block>();
        Chain.Add(CreateGenesisBlock());

        Nodes = new List<string>();

        Difficulty = 2;
        PendingTransactions = new List<Transaction>();
    }

    private Block CreateGenesisBlock()
    {
        Block genesis = new Block(new DateTime(2000, 01, 01), new List<Transaction>(), "0");
        return genesis;
    }
}