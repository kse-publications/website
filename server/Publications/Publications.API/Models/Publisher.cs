using Redis.OM.Modeling;

namespace Publications.API.Models;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "publisher" })]
public class Publisher
{
    [RedisIdField]
    [Indexed]
    public Guid Id { get; set; }
    
    [Searchable]
    public string Name { get; set; } = null!;
}