using Publications.Domain.Publications;
using Publications.Domain.Shared;
using Publications.Domain.Shared.Slugs;
using Publications.Domain.Shared.ValueObjects;
using Redis.OM.Modeling;

namespace Publications.Domain.Authors;

/// <summary>
/// Represents an author of a <see cref="Publication"/>.
/// </summary>
[Document (IndexName = "author-idx", StorageType = StorageType.Json, Prefixes = ["author"])]
public class Author: Entity<Author>
{
    [Searchable(Weight = 1.0, PhoneticMatcher = "dm:en")]
    public string Name { get; init; } = null!;
    
    [Searchable(Weight = 0.6)]
    public string ProfileLink { get; set; } = string.Empty;
    
    public override Author UpdateSlug(IWordsService wordsService)
    {
        Slug = SlugService.Create(
            Name, Id.ToString(), IsoLanguageCode.English, wordsService);
        
        return this;
    }
}