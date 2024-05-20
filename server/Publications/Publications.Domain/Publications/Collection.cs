using Redis.OM.Modeling;

namespace Publications.Domain.Publications;

/// <summary>
/// Represents a logically grouped set of publications.
/// </summary>
[Document(IndexName = "collection-idx", StorageType = StorageType.Json, Prefixes = ["collection"])]
public class Collection
{
    [RedisIdField]
    [Indexed(Sortable = true)]
    public int Id { get; set; }
    
    [Indexed]
    public string Icon { get; set; } = string.Empty;
    
    [Searchable(Weight = 1.0)]
    public string Name { get; set; } = null!;
    
    [Searchable(Weight = 0.8)]
    public string Description { get; set; } = string.Empty;
    
    [Indexed(Sortable = true)]
    public int PublicationsCount { get; set; }
}