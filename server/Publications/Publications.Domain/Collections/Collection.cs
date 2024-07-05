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
public class Collection: Entity
{
    [Indexed] 
    [JsonInclude]
    public string Icon { get; set; } = string.Empty;

    [Searchable(Weight = 1.0)] 
    public string Name { get; init; }

    [Searchable(Weight = 0.8)]
    [JsonInclude]
    public string Description { get; set; } = string.Empty;
    
    [Indexed]
    [IgnoreInResponse] 
    [JsonInclude]
    public string PublicationsIds { get; set; } = string.Empty;
    
    [Indexed]
    [IgnoreInResponse]
    [JsonInclude]
    public string[] Keywords { get; set; } = Array.Empty<string>();
    
    [Indexed]
    [IgnoreInResponse]
    [JsonInclude]
    public int[] IgnoredPublicationIds { get; set; } = Array.Empty<int>();
    
    [JsonConstructor]
    public Collection(
        int id,
        string name, 
        DateTime lastModifiedAt)
    {
        Id = id;
        Name = name;
        LastSynchronizedAt = DateTime.UtcNow;
        LastModifiedAt = lastModifiedAt;
    }
    
    public Collection HydrateSlug(IWordsService wordsService)
    {
        Slug = SlugFactory.Create(Name, Id.ToString(), IsoLanguageCode.English, wordsService);
        return this;
    }
    
    public int[] GetPublicationIds() => ParsePublicationIds(PublicationsIds);
    
    public static int[] ParsePublicationIds(string publicationIds)
    {
        return publicationIds.Split(',').Select(int.Parse).ToArray();
    }

    protected void SetPublicationIds(int[] publicationIds)
    {
        PublicationsIds = string.Join(',', publicationIds);
    }
}