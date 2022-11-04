using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;

namespace BlockchainBack.Models;

public class Transaction
{
    
    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    
    public string? Id { get; set; }
    public Operation Sender { get; set; }
    public Operation Receiver { get; set; }
    public decimal Amount { get; set; }
    //public string Description { get; set; }
    
    
    public ObjectId BlockId { get; set; }
    
    public ObjectId BlockchainId { get; set; }
        
    public Transaction(Operation sender, Operation receiver, decimal amount)
    {
        Sender = sender;
        Receiver = receiver;
        Amount = amount; 
        //Description = description;
    }

}