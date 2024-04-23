using Publications.API.Slugs;
using Redis.OM.Modeling;

namespace Publications.API.Models;

/// <summary>
/// Represents a publisher of a <see cref="Publication"/>. 
/// </summary>
[Document(IndexName = "publisher-idx",StorageType = StorageType.Json, Prefixes = ["publisher"])]
public class Publisher: Entity<Publisher>
{
    [Searchable(Weight = 1.0, PhoneticMatcher = "dm:en")]
    public string Name { get; set; } = null!;
    
    public override Publisher UpdateSlug()
    {
        Slug = SlugGenerator.GenerateSlug(
            Name, Id.ToString(), LanguageService.English);
        
        return this;
    }   
}