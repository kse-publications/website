using System.Security.Cryptography;
using System.Text;
using Redis.OM.Modeling;

namespace Publications.Domain.Publications;

public class Filter
{
    [Indexed]
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;
    
    public int MatchedPublicationsCount { get; set; }
    
    public static Filter Create(string value)
    {
        return new Filter
        {
            Id = GenerateIdFromValue(value),
            Value = value
        };
    }

    private static int GenerateIdFromValue(string value)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(value);
        int filterId = BitConverter.ToInt32(sha256.ComputeHash(inputBytes), 0);

        return Math.Abs(filterId);
    }
}