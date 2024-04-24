using System.Security.Cryptography;
using System.Text;

namespace Publications.API.Services;

public class IdGenerator
{
    public static int GenerateFromValue(string value)
    {
        byte[] hashBytes = ComputeHash(value);
        int filterId = BitConverter.ToInt32(hashBytes, 0);

        return Math.Abs(filterId);
    }

    private static byte[] ComputeHash(string input)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        return sha256.ComputeHash(inputBytes);
    }
}