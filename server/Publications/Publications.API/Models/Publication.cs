using Redis.OM.Modeling;

namespace Publications.API.Models;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "publication" })]
public class Publication
{
    [RedisIdField]
    [Indexed]
    public Guid Id { get; set; }
    
    [Searchable]
    public string Title { get; set; }
    
    [Indexed]
    public string Type { get; set;}
    
    [Indexed]
    public int Year { get; }
    
    public string Link { get; }
    
    [Searchable]
    public string[] Keywords { get; }
    
    [Searchable]
    public string Abstract { get; }
    
    public ICollection<Author> Authors { get; }
    
    public Publisher Publishers { get; }
}