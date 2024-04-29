using System.Security.Cryptography;
using System.Text;
using Redis.OM.Modeling;

namespace Publications.Domain.Filters;

public class Filter
{
    [Indexed]
    public int Id { get; private set; }

    private readonly string _value = null!;
    public string Value
    {
        get => _value; 
        init 
        {
            _value = value;
            Id = GenerateIdFromValue(value);
        }
    }
    
    private static int GenerateIdFromValue(string value)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(value);
        int filterId = BitConverter.ToInt32(sha256.ComputeHash(inputBytes), 0);

        return Math.Abs(filterId);
    }
}