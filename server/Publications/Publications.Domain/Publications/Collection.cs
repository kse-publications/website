using Publications.Domain.Shared;
using Publications.Domain.Shared.Slugs;
using Publications.Domain.Shared.ValueObjects;
using Redis.OM.Modeling;

namespace Publications.Domain.Publications;

/// <summary>
/// Represents a logically grouped set of publications.
/// </summary>
[Document(IndexName = "collection-idx", StorageType = StorageType.Json, Prefixes = ["collection"])]
public class Collection: Entity<Collection>
{
    [Indexed] 
    public string Icon { get; set; } = string.Empty;

    [Searchable(Weight = 1.0)] 
    public string Name { get; set; } = null!;

    [Searchable(Weight = 0.8)] 
    public string Description { get; set; } = string.Empty;

    [Indexed(Sortable = true)] 
    public int PublicationsCount { get; set; }

    public override Collection UpdateSlug(IWordsService wordsService)
    {
        Slug = SlugService.Create(Name, Id.ToString(), IsoLanguageCode.English, wordsService);
        return this;
    }
}