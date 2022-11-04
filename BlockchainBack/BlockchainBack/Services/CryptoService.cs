using System.Security.Cryptography;

namespace BlockchainBack.Services;

public class CryptoService
{
    public static byte[] GetHash(byte[] input)
    {
        using var algorithm = SHA256.Create();
        var hashBytes = algorithm.ComputeHash(input);

        return hashBytes;
    }
}