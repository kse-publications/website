using System.Text.Json.Serialization;
using Publications.Domain.Shared;
using Publications.Domain.Shared.Serialization;
using Publications.Domain.Shared.Slugs;
using Publications.Domain.Shared.ValueObjects;
using Redis.OM.Modeling;

namespace Publications.Domain.Collections;

/// <summary>
/// Represents a logically grouped set of publications.
/// </summary>
[Document(IndexName = "collection-idx", StorageType = StorageType.Json, Prefixes = ["collection"])]
public class Collection: Entity<Collection>
{
    [Indexed] 
    [JsonInclude]
    public string Icon { get; set; } = string.Empty;

    [Searchable(Weight = 1.0)] 
    public string Name { get; init; }

    [Searchable(Weight = 0.8)]
    [JsonInclude]
    public string Description { get; set; } = string.Empty;
    
    [IgnoreInResponse]
    public int[] PublicationsIds { get; set; } = Array.Empty<int>();

    [Indexed(Sortable = true)]
    [JsonInclude]
    public int PublicationsCount { get; set; }

    [JsonConstructor]
    public Collection(
        int id,
        string name, 
        DateTime lastModifiedAt,
        IWordsService wordsService)
    {
        Id = id;
        Name = name;
        LastSynchronizedAt = DateTime.UtcNow;
        LastModifiedAt = lastModifiedAt;
        
        UpdateSlug(wordsService);
    }

    private Collection UpdateSlug(IWordsService wordsService)
    {
        Slug = SlugFactory.Create(Name, Id.ToString(), IsoLanguageCode.English, wordsService);
        return this;
    }
}