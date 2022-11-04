using BlockchainBack.Models;

namespace BlockchainBack.Services;

public class NodeServices
{
    
    private readonly Blockchain _blockchain;
    private readonly MongoDbRepository _mongoDbRepository = new MongoDbRepository();
    public List<string> Nodes
    {
        get
        {
            return _blockchain.Nodes;
        }
    }

    public NodeServices(Blockchain blockchain)
    {
        _blockchain = blockchain;
    }


    public void AddNode(string nodeUrl)
    {
        if (_blockchain.Nodes.Contains(nodeUrl)) return;
        _mongoDbRepository.CreateNode(nodeUrl,_blockchain);
        _blockchain.Nodes.Add(nodeUrl);
    }

    public void RemoveNode(string nodeUrl)
    {
        _mongoDbRepository.RemoveNode(nodeUrl, _blockchain);
        _blockchain.Nodes.Remove(nodeUrl);
    }
}