using Publications.API.Services;
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
    public int Id { get; set; }
    
    public Guid NotionId { get; set; }
    public string Slug { get; set; } = null!;
    
    [Searchable(Weight = 1.0, PhoneticMatcher = "dm:en")]
    public string Name { get; set; } = null!;
    
    public Publisher UpdateSlug()
    {
        Slug = SlugService.GenerateSlug(Name, Id.ToString());
        return this;
    }   
}