using BlockchainBack.Models;
using MongoDB.Bson;

namespace BlockchainBack.Services;

public class BlockchainServices
{
    public Blockchain Blockchain { get; set; }
    public object? Program { get; private set; }
    private readonly MongoDbRepository _mongoDbRepository = new MongoDbRepository();

    public BlockchainServices(Blockchain blockchain)
    {
        Blockchain = blockchain;
        //    blockchain = new Blockchain.Blockchain();
        // calculate hash of genesis block

        if (blockchain.Chain[0].Hash != "") return;
        var genesisBlock = blockchain.Chain[0];
        var blockServices = new BlockServices(genesisBlock);
        var genesisBlockHash = blockServices.BlockHash();

        blockchain.Chain[0].Hash = genesisBlockHash;
    }


    public void UpdateWithLongestBlockchain()
    {
        var longestBlockchainNode = "";
        var maxBlockchainLength = 0;
    }

    private Block LatestBlock()
    {
        return Blockchain.Chain.Last();
    }

    public Block Block(int index)
    {
        return Blockchain.Chain[index];
    }

    public int BlockchainLength()
    {
        return Blockchain.Chain.Count;
    }

    public void AddTransaction(Transaction transaction)
    {
        Console.WriteLine("ASDASD=> " + Blockchain.Id);
        _mongoDbRepository.CreateTransactionInBlockchain(transaction, Blockchain);
        Blockchain.PendingTransactions.Add(transaction);
    }

    public List<Transaction> PendingTransactions() => Blockchain.PendingTransactions;

    public async Task<Block> MineBlock()
    {
        // add mining reward transaction to block
        //Transaction trans = new Transaction("SYSTEM", miningRewardAddress, blockchain.MiningReward, "Mining reward");
        //blockchain.PendingTransactions.Add(trans);
        Blockchain = await _mongoDbRepository.UpdateBlockchain();
        var block = new Block(DateTime.Now, Blockchain.PendingTransactions, LatestBlock().Hash);
        var blockServices = new BlockServices(block);
        blockServices.MineBlock(Blockchain.Difficulty);
        await _mongoDbRepository.MineBlock(block, Blockchain);
        Blockchain.Chain.Add(block);
        //clear pending transactions (all pending transactions are in a block
        Blockchain.PendingTransactions = new List<Transaction>();
        return block;
    }

    public bool IsBlockchainValid()
    {
        for (long i = 1; i < Blockchain.Chain.Count(); i++)
        {
            var currentBlock = Blockchain.Chain[(int)i];
            var previousBlock = Blockchain.Chain[(int)i - 1];

            var blockServices = new BlockServices(currentBlock);
            if (currentBlock.Hash != blockServices.BlockHash())
            {
                return false;
            }

            if (currentBlock.PreviousHash != previousBlock.Hash)
            {
                return false;
            }
        }

        return true;
    }

    public decimal Balance(string address)
    {
        decimal balance = 0;

        foreach (var transaction in Blockchain.Chain.SelectMany(block => block.Transactions))
        {
            if (transaction.Sender.Address == address)
            {
                balance -= transaction.Amount;
            }

            if (transaction.Receiver.Address == address)
            {
                balance += transaction.Amount;
            }
        }

        return balance;
    }

    public void DeleteTransaction(string id)
    {
        _mongoDbRepository.DeleteTransaction(id, Blockchain);
        Blockchain.PendingTransactions.RemoveAll(transaction => transaction.Id == id);
    }
}