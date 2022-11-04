using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace BlockchainBack.Models;


public class Block
{
    public ObjectId Id { get; set; }

    public string PreviousHash { get; set; }
    public DateTime TimeStamp { get; set; }
    public List<Transaction> Transactions { get; set; }
    public string Hash { get; set; }
    public long Nonce { get; set; }

   
    public ObjectId BlockchainId { get; set; }

    public Block(DateTime timeStamp, List<Transaction> transactions, string previousHash)
    {
        TimeStamp = timeStamp;
        PreviousHash = previousHash;
        Transactions = transactions;
        Hash = "";
        Nonce = 0;
    }
}