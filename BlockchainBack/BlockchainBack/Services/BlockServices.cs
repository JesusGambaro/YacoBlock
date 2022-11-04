using System.Text;
using BlockchainBack.Models;

namespace BlockchainBack.Services;

public class BlockServices
{
    private readonly Block _block;

    public BlockServices(Block block)
    {
        _block = block;
    }

    private string ByteArrayToString(byte[] ba)
    {
        return BitConverter.ToString(ba).Replace("-", "");
    }

    public string BlockHash()
    {
        //string hashString = block.PreviousHash + block.TimeStamp.ToString() + JsonConvert.SerializeObject(block.Transactions.ToList(), Formatting.Indented) + block.Nonce;
        var hashString = _block.PreviousHash + _block.TimeStamp.ToString("") + _block.Nonce;
        var hashBytes = Encoding.ASCII.GetBytes(hashString);
        var hashResult = CryptoService.GetHash(hashBytes);
        return ByteArrayToString(hashResult);
    }

    public void MineBlock(int difficulty)
    {
        var startsWith = "";
        for (var n = 0; n < difficulty; n++)
        {
            startsWith += "0";
        }

        while (_block.Hash == "" || _block.Hash[..difficulty] != startsWith)
        {
            _block.Nonce += 1;
            _block.Hash = BlockHash();
        }
    }
}