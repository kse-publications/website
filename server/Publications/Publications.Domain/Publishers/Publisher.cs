using Publications.Domain.Publications;
using Publications.Domain.Shared;
using Publications.Domain.Shared.Slugs;
using Publications.Domain.Shared.ValueObjects;
using Redis.OM.Modeling;

namespace Publications.Domain.Publishers;

/// <summary>
/// Represents a publisher of a <see cref="Publication"/>. 
/// </summary>
[Document(IndexName = "publisher-idx",StorageType = StorageType.Json, Prefixes = ["publisher"])]
public class Publisher: Entity<Publisher>
{
    [Searchable(Weight = 1.0, PhoneticMatcher = "dm:en")]
    public string Name { get; init; } = null!;
    
    public override Publisher UpdateSlug(IWordsService wordsService)
    {
        Slug = SlugService.Create(
            Name, Id.ToString(), IsoLanguageCode.English, wordsService);
        
        return this;
    }   
}