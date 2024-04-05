using Publications.API.Slugs;
using Redis.OM.Modeling;

namespace Publications.API.Models;

/// <summary>
/// Represents an author of a <see cref="Publication"/>.
/// </summary>
[Document (IndexName = "publication-idx", StorageType = StorageType.Json, Prefixes = ["author"])]
public class Author: Entity<Author>
{
    [Searchable(Weight = 1.0, PhoneticMatcher = "dm:en")]
    public string Name { get; set; } = null!;
    
    [Searchable(Weight = 0.6)]
    public string ProfileLink { get; set; } = string.Empty;
    
    public override Author UpdateSlug()
    {
        Slug = SlugGenerator.GenerateSlug(
            Name, Id.ToString(), LanguageService.English);
        
        return this;
    }
}