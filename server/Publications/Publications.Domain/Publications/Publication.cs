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
    [JsonInclude]
    [IgnoreInResponse]
    public Vector<string>? SimilarityVector { get; set; }
    
    [Indexed(Sortable = true)]
    public string Type { get; init; }
    
    [Indexed(Sortable = true)]
    [JsonInclude]
    public string Language { get; set; } = string.Empty;
    
    [Indexed(Sortable = true)]
    public int Year { get; init; }
    
    public string Link { get; init; }
    
    [Searchable(JsonPath = "$.Value", Weight = 0.8)]
    [JsonInclude]
    [IgnoreInResponse]
    public Keyword[] SearchableKeywords { get; set; } = Array.Empty<Keyword>();
    
    public string[] Keywords => SearchableKeywords.Select(k => k.Value).ToArray();

    [Searchable(Weight = 0.6)] 
    public string AbstractText { get; init; }
    
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
    [IgnoreInResponse]
    public Collection[] Collections { get; set; } = Array.Empty<Collection>();
    
    [Indexed(Sortable = true)]
    [JsonInclude]
    [IgnoreInResponse]
    public bool Vectorized { get; set; }
    
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
        AbstractText = abstractText;
        LastModifiedAt = lastModifiedAt;
        LastSynchronizedAt = DateTime.UtcNow;
    }
    
    public Publication HydrateSlug(IWordsService wordsService)
    {
        Slug = SlugFactory.Create(
            Title, Id.ToString(), IsoLanguageCode.Create(Language), wordsService);
        
        return this;
    }
    
    public void Vectorize(IWordsService wordsService)
    {
        SimilarityVector = Vector.Of(GetSimilarityValue()
            .ToLower()
            .RemoveStopWords(IsoLanguageCode.Create(Language), wordsService)
            .Transliterate(wordsService)
            .RemoveSpecialChars());
        
        Vectorized = true;
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
        nameof(AbstractText),
        $"{nameof(SearchableKeywords)}_{nameof(Keyword.Value)}",
        $"{nameof(Authors)}_{nameof(Author.Name)}",
        $"{nameof(Publisher)}_{nameof(Publisher.Name)}",
        $"{nameof(Collections)}_{nameof(Collection.Name)}"
    ];

    public string GetSimilarityValue() =>
        string.Join(" ", Enumerable.Repeat(Title, 2)) + " " +
        string.Join(" ", Keywords) + " " +
        string.Join(" ", Collections.Select(c => c.Name)) + " " +
        AbstractText;
}