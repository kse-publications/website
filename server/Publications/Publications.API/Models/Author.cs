
using System.Text.Json.Serialization;
using Publications.API.Services;
using Redis.OM.Modeling;

namespace Publications.API.Models;

/// <summary>
/// Represents an author of a <see cref="Publication"/>.
/// </summary>
[Document (IndexName = "publication-idx", StorageType = StorageType.Json, Prefixes = ["author"])]
public class Author
{
    [RedisIdField]
    [Indexed]
    public int Id { get; set; }
    
    [JsonIgnore]
    public Guid NotionId { get; set; }
    public string Slug { get; set; } = null!;
    
    [Searchable(Weight = 1.0, PhoneticMatcher = "dm:en")]
    public string Name { get; set; } = null!;
    
    [Searchable(Weight = 0.6)]
    public string ProfileLink { get; set; } = string.Empty;
    
    public Author UpdateSlug()
    {
        Slug = SlugService.GenerateSlug(Name, Id.ToString());
        return this;
    }
}