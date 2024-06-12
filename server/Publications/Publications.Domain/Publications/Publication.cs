using System.Text.Json.Serialization;
using Publications.Domain.Collections;
using Publications.Domain.Filters;
using Publications.Domain.Shared;
using Publications.Domain.Shared.Serialization;
using Publications.Domain.Shared.Slugs;
using Publications.Domain.Shared.ValueObjects;
using Redis.OM;
using Redis.OM.Modeling;
using Redis.OM.Vectorizers.AllMiniLML6V2;

namespace Publications.Domain.Publications;

/// <summary>
/// Aggregate root of Publications aggregate.
/// </summary>
[Document(IndexName = "publication-idx", StorageType = StorageType.Json, Prefixes = ["publication"])]
public class Publication: Entity
{
    [Searchable(Weight = 1.0)]
    public string Title { get; init; }

    [Indexed(DistanceMetric = DistanceMetric.COSINE, Algorithm = VectorAlgorithm.HNSW)]
    [SentenceVectorizer]
    [IgnoreInResponse]
    [JsonInclude]
    public Vector<string> SimilarityVector { get; set; } = null!;
    
    [Indexed(Sortable = true)]
    public string Type { get; init; }
    
    [Indexed(Sortable = true)]
    [JsonInclude]
    public string Language { get; set; } = string.Empty;
    
    [Indexed(Sortable = true)]
    public int Year { get; init; }
    
    public string Link { get; init; }
    
    [Searchable(Weight = 0.8)]
    [JsonInclude]
    public string[] Keywords { get; set; } = Array.Empty<string>();

    [Searchable(Weight = 0.7)] 
    public string Abstract { get; init; }
    
    [Indexed(JsonPath = "$.Id")]
    [Searchable(JsonPath = "$.Name", Weight = 0.8, PhoneticMatcher = "dm:en")]
    [JsonInclude]
    public Author[] Authors { get; set; } = Array.Empty<Author>();
    
    [Indexed(JsonPath = "$.Id")]
    [Searchable(JsonPath = "$.Name", Weight = 0.8, PhoneticMatcher = "dm:en")]
    [JsonInclude]
    public Publisher? Publisher { get; set; }
    
    [Indexed(Sortable = true)]
    [JsonInclude]
    public int Views { get; set; } 
    
    [Indexed(Sortable = true)]
    [JsonInclude]
    public int RecentViews { get; set; }
    
    [Indexed(JsonPath = "$.Id")]
    [IgnoreInResponse]
    [JsonInclude]
    public Filter[] Filters { get; set; } = Array.Empty<Filter>();
    
    [Indexed(JsonPath = "$.Id")]
    [Searchable(JsonPath = "$.Name", Weight = 0.8)]
    [JsonInclude]
    public Collection[] Collections { get; set; } = Array.Empty<Collection>();
    
    private Publication(int id) { Id = id; }
    
    [JsonConstructor]
    public Publication( 
        int id,
        string title, 
        string type, 
        int year, 
        string link,
        string abstractText,
        DateTime lastModifiedAt)
    {
        Id = id;
        Title = title;
        Type = type;
        Year = year;
        Link = link;
        Abstract = abstractText;
        LastModifiedAt = lastModifiedAt;
        LastSynchronizedAt = DateTime.UtcNow;
    }

    public static Publication InitWithId(int id) => new(id);
    
    public Publication HydrateDynamicFields(IWordsService wordsService)
    {
        UpdateSlug(wordsService);
        UpdateVectors(wordsService);
        return this;
    }
    
    private void UpdateSlug(IWordsService wordsService)
    {
        Slug = SlugFactory.Create(
            Title, Id.ToString(), IsoLanguageCode.Create(Language), wordsService);
    }
    
    private void UpdateVectors(IWordsService wordsService)
    {
        SimilarityVector = Vector.Of(wordsService
            .Transliterate(GetSimilarityValue())
            .RemoveSpecialChars());
    }
    
    public static EntityFilter[] GetEntityFilters() =>
    [
        new EntityFilter(groupId: 1, nameof(Type)),
        new EntityFilter(groupId: 2, nameof(Year), SortOrder.Descending),
        new EntityFilter(groupId: 3, nameof(Language))
    ];
    
    public static string[] GetSearchableFields() =>
    [
        nameof(Title),
        nameof(Abstract),
        $"{nameof(Authors)}_{nameof(Author.Name)}",
        $"{nameof(Publisher)}_{nameof(Publisher.Name)}"
    ];
    
    public static string GetKey(int id) => $"publication:{id}";

    private string GetSimilarityValue() =>
        Title + " " + 
        Abstract + " " +
        string.Join(" ", Keywords) + " " + 
        string.Join(" ", Collections.Select(c => c.Name));
}