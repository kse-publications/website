using Redis.OM.Modeling;

namespace Publications.API.Models;

/// <summary>
/// Represents a publisher of a <see cref="Publication"/>. 
/// </summary>
[Document(IndexName = "publication-idx",StorageType = StorageType.Json, Prefixes = ["publisher"])]
public class Publisher
{
    [RedisIdField]
    [Indexed]
    public Guid Id { get; set; }
    
    [Searchable(Weight = 1.0, PhoneticMatcher = "dm:en")]
    public string Name { get; set; } = null!;
}