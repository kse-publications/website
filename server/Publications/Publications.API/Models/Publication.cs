using Redis.OM.Modeling;

namespace Publications.API.Models;

/// <summary>
/// Aggregate root that represents a publication.
/// </summary>
[Document(StorageType = StorageType.Json, Prefixes = new[] { "publication" })]
public class Publication
{
    [RedisIdField]
    [Indexed]
    public Guid Id { get; set; }
    
    [Searchable]
    public string Title { get; set; } = null!;
    
    [Indexed(Sortable = true)]
    public string Type { get; set;} = null!;
    
    [Indexed(Sortable = true)]
    public int Year { get; set; }
    
    public string Link { get; set; } = null!;
    
    [Searchable]
    public string[] Keywords { get; set; } = Array.Empty<string>();

    [Searchable] 
    public string Abstract { get; set; } = string.Empty;
    
    public ICollection<Author> Authors { get; set; } = new List<Author>();
    
    [Indexed(CascadeDepth = 1)]
    public Publisher? Publisher { get; set; }
    
    [Indexed(Sortable = true)]
    public DateTime LastModified { get; set; }
}