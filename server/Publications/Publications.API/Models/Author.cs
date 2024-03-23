using Redis.OM.Modeling;

namespace Publications.API.Models;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "author" })]
public class Author
{
    [RedisIdField]
    [Indexed]
    public Guid Id { get; set; }
    
    [Indexed]
    public string Name { get; set; } = null!;
    
    public string ProfileLink { get; set; } = string.Empty;
}