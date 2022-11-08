using BlockchainBack.Controllers;
using BlockchainBack.Models;
using BlockchainBack.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BlockchainBack;

public class MongoDbRepository
{
    private readonly IMongoDatabase _user1;

    public MongoDbRepository()
    {
        var client = new MongoClient(
            "mongodb+srv://admin:admin@cluster0.h2jl7dy.mongodb.net/?retryWrites=true&w=majority");
        _user1 = client.GetDatabase("user1");
    }

    private async Task CreateBlockchain(Blockchain blockchain)
    {
        //Insert blockchain and blocks into database linkin blocks to blockchain
        var collection = _user1.GetCollection<Blockchain>("blockchain");

        await collection.InsertOneAsync(blockchain);

        var collection2 = _user1.GetCollection<Block>("block");
        await collection2.InsertManyAsync(blockchain.Chain);

        var blocksIds = new List<Block>();
        foreach (var block in blockchain.Chain)
        {
            var filter = Builders<Block>.Filter.Eq("Hash", block.Hash);
            var update = Builders<Block>.Update.Set("BlockchainId", blockchain.Id);
            await collection2.UpdateOneAsync(filter, update);
            block.BlockchainId = blockchain.Id;
            blocksIds.Add(block);
        }

        //Set id of blocks in each block in blockchain
        var filter2 = Builders<Blockchain>.Filter.Eq("Id", blockchain.Id);
        var update2 = Builders<Blockchain>.Update.Set("Chain", blocksIds);
        await collection.UpdateOneAsync(filter2, update2);
    }

    public async Task<Blockchain> GetBlockchain()
    {
        //Get the blockchain, blocks and transactions from the database
        //if the database is empty, create a new blockchain
        var collection = _user1.GetCollection<Blockchain>("blockchain");
        var collection2 = _user1.GetCollection<Block>("block");
        var collection3 = _user1.GetCollection<Transaction>("transaction");

        var blockchain = await collection.Find(_ => true).FirstOrDefaultAsync();

        if (blockchain == null)
        {
            Console.WriteLine("Blockchain is null");
            var blockchainServices = new BlockchainServices(new Blockchain());
            blockchain = blockchainServices.Blockchain;
            await CreateBlockchain(blockchain);
        }
        else
        {
            Console.WriteLine("Blockchain is not null");
            //Get blocks by matching blockchain id and matching block id with each block in the blockchain
            await UpdateBlockchain();
            //blocks.ForEach(bk => Console.WriteLine(bk.Hash));
            Console.ResetColor();
        }

        return blockchain;
    }

    public async Task<Blockchain> UpdateBlockchain()
    {
        //Get the blockchain, blocks and transactions from the database
        var collection = _user1.GetCollection<Blockchain>("blockchain");
        var collection2 = _user1.GetCollection<Block>("block");
        var collection3 = _user1.GetCollection<Transaction>("transaction");

        var blockchain = await collection.Find(_ => true).FirstOrDefaultAsync();
        //Get blocks by matching blockchain id and matching block id with each block in the blockchain
        var blocks = await collection2.Find(b => b.BlockchainId == blockchain.Id).ToListAsync();
        blockchain.Chain = blocks;
        //Get transactions by matching block id and matching transaction id with each transaction in the block
        foreach (var block in blockchain.Chain)
        {
            var transactions = await collection3.Find(t => t.BlockId == block.Id).ToListAsync();
            block.Transactions = transactions;
        }

        //get pending transactions by matching blockchain id
        var pendingTransactions = await collection3.Find(t => t.BlockchainId == blockchain.Id).ToListAsync();
        blockchain.PendingTransactions = pendingTransactions;
        return blockchain;
    }

    private async Task UpdateBlock(Block block)
    {
        var collection = _user1.GetCollection<Block>("block");
        var filter = Builders<Block>.Filter.Eq("Hash", block.Hash);
        var update = Builders<Block>.Update.Set("Transactions", block.Transactions);
        await collection.UpdateOneAsync(filter, update);
    }

    private async Task UpdateTransaction(Transaction transaction)
    {
        //Update the transaction in the database
        var trCollection = _user1.GetCollection<Transaction>("transaction");
        //update BlockId and BlockchainId
        var filter = Builders<Transaction>.Filter.Eq("Id", transaction.Id);
        var update = Builders<Transaction>.Update.Set("BlockId", transaction.BlockId);
        var update2 = Builders<Transaction>.Update.Set("BlockchainId", transaction.BlockchainId);
        await trCollection.UpdateOneAsync(filter, update);
        await trCollection.UpdateOneAsync(filter, update2);
    }

    public async Task CreateTransaction(Transaction transaction)
    {
        //Insert transaction into database
        var collection = _user1.GetCollection<Transaction>("transaction");
        await collection.InsertOneAsync(transaction);
    }

    public async Task MineBlock(Block block, Blockchain blockchain)
    {
        //Insert block into database
        block.Transactions = blockchain.PendingTransactions;
        //blockchain.PendingTransactions.ForEach(pt=>Console.WriteLine(pt.Description));
        block.BlockchainId = blockchain.Id;
        var bkCollection = _user1.GetCollection<Block>("block");
        await bkCollection.InsertOneAsync(block);
        foreach (var transaction in block.Transactions)
        {
            transaction.BlockId = block.Id;
            transaction.BlockchainId = new ObjectId();
            //Console.WriteLine(transaction.BlockchainId);
            //Console.WriteLine(transaction.BlockId);
            await UpdateTransaction(transaction);
        }

        //Link block to blockchain
        var bcCollection = _user1.GetCollection<Blockchain>("blockchain");
        //Delete pending transactions from blockchain and add them to the block


        var bcFilter = Builders<Blockchain>.Filter.Eq("Id", blockchain.Id);


        //update block transactions
        await UpdateBlock(block);
        var ptUpdate = Builders<Blockchain>.Update.Set("PendingTransactions", new List<Transaction>());
        await bcCollection.UpdateOneAsync(bcFilter, ptUpdate);
        //Add block to blockchain
        var chUpdate = Builders<Blockchain>.Update.Push("Chain", block);
        await bcCollection.UpdateOneAsync(bcFilter, chUpdate);
    }

    public async Task CreateTransactionInBlock(Transaction transaction, Block block)
    {
        //Insert transaction into block and update the block in the database
        var collection = _user1.GetCollection<Block>("block");
        var filter = Builders<Block>.Filter.Eq("Hash", block.Hash);
        var update = Builders<Block>.Update.Push("Transactions", transaction);
        await collection.UpdateOneAsync(filter, update);
    }

    public async void CreateTransactionInBlockchain(Transaction transaction)
    {
        //Insert transaction into blockchain and update the blockchain in the database
        var blockChainData = GetBlockchain();
        blockChainData.Wait();
        Console.WriteLine("Blockchain loaded 1 " + blockChainData.Result.Id);
        //Console.WriteLine("Blockchain loaded 2 " + blockchain.Id);
        transaction.BlockchainId = blockChainData.Result.Id;
        var trCollection = _user1.GetCollection<Transaction>("transaction");
        await trCollection.InsertOneAsync(transaction);
        var collection = _user1.GetCollection<Blockchain>("blockchain");
        var filter = Builders<Blockchain>.Filter.Eq("Id", blockChainData.Result.Id);
        var update = Builders<Blockchain>.Update.Push("PendingTransactions", transaction);
        await collection.UpdateOneAsync(filter, update);
    }

    public async Task CreateBlockInBlockchain(Block block, Blockchain blockchain)
    {
        //Insert block into blockchain and update the blockchain in the database
        var collection = _user1.GetCollection<Blockchain>("blockchain");
        var filter = Builders<Blockchain>.Filter.Eq("Id", blockchain.Id);
        var update = Builders<Blockchain>.Update.Push("Chain", block);
        await collection.UpdateOneAsync(filter, update);
    }

    public async void DeleteAll()
    {
        //Delete all data from the database
        var collection = _user1.GetCollection<Blockchain>("blockchain");
        var collection2 = _user1.GetCollection<Block>("block");
        var collection3 = _user1.GetCollection<Transaction>("transaction");
        await collection.DeleteManyAsync(new BsonDocument());
        await collection2.DeleteManyAsync(new BsonDocument());
        await collection3.DeleteManyAsync(new BsonDocument());
    }

    public async void CreateNode(string node, Blockchain blockchain)
    {
        //Insert node into blockchain and update the blockchain in the database
        var bcCollection = _user1.GetCollection<Blockchain>("blockchain");
        var filter = Builders<Blockchain>.Filter.Eq("Id", blockchain.Id);
        var update = Builders<Blockchain>.Update.Push("Nodes", node);
        await bcCollection.UpdateOneAsync(filter, update);
    }

    public async void RemoveNode(string node, Blockchain blockchain)
    {
        //Remove node from blockchain and update the blockchain in the database
        var bcCollection = _user1.GetCollection<Blockchain>("blockchain");
        var filter = Builders<Blockchain>.Filter.Eq("Id", blockchain.Id);
        var update = Builders<Blockchain>.Update.Pull("Nodes", node);
        await bcCollection.UpdateOneAsync(filter, update);
    }

    public void DeleteTransaction(string id, Blockchain blockchain)
    {
        //Delete transaction from blockchain and update the blockchain in the database
        var bcCollection = _user1.GetCollection<Blockchain>("blockchain");
        var trCollection = _user1.GetCollection<Transaction>("transaction");
        var tr = trCollection.Find(t => t.Id == id).FirstOrDefault();
        var filter = Builders<Blockchain>.Filter.Eq("Id", blockchain.Id);

        var update = Builders<Blockchain>.Update.Pull("PendingTransactions", tr);

        bcCollection.UpdateOne(filter, update);
        trCollection.DeleteOne(t => t.Id == id);
    }

    public int GetChainLength()
    {
        //Get the length of the chain
        var collection = _user1.GetCollection<Blockchain>("blockchain");
        var chain = collection.Find(new BsonDocument()).FirstOrDefault();
        
        if (chain == null)
        {
            Console.WriteLine("There is no blockchain");
            return 0;
        }
        return chain.Chain.Count;
    }

    public Task<List<LibroMayor>> GetTransactionsMapped(BlockchainServices blockchainServices)
    {
        var bkCollection = _user1.GetCollection<Block>("block");

        var bk = bkCollection.Find(_ => true).ToList();
        //save all transactions of all blocks in a list
        var transactions = bk.SelectMany(block => block.Transactions).ToList();
        var librosMayores = new List<LibroMayor>();
        //unique list of senders and receivers
        var senders = transactions.Select(t => t.Sender.Address).Distinct().ToList();
        var receivers = transactions.Select(t => t.Receiver.Address).Distinct().ToList();
        //list of unique senders and receivers
        var uniqueSenders = receivers.Union(senders).ToList();
        //create a list of LibroMayor with the unique senders and receivers
        foreach (var den in uniqueSenders)
        {
            var libro = new LibroMayor();
            var senderTransactions = transactions.Where(t => t.Sender.Address == den).ToList();
            var receiverTransactions = transactions.Where(t => t.Receiver.Address == den).ToList();
            libro.Cuenta = den;
            libro.Debe = senderTransactions.Sum(t => t.Amount);
            libro.DebeList = senderTransactions.Select(t => t.Amount).ToList();
            libro.Haber = receiverTransactions.Sum(t => t.Amount);
            libro.HaberList = receiverTransactions.Select(t => t.Amount).ToList();
            libro.Saldo = blockchainServices.GetBalance(den);
            librosMayores.Add(libro);
        }

        return Task.FromResult(librosMayores);
    }
}