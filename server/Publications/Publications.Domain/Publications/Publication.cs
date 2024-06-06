﻿using Publications.Domain.Collections;
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
public class Publication: Entity<Publication>
{
    [Searchable(Weight = 1.0)]
    public string Title { get; set; } = null!;
    
    [Indexed(DistanceMetric = DistanceMetric.COSINE, Algorithm = VectorAlgorithm.HNSW)]
    [SentenceVectorizer]
    [IgnoreInResponse]
    public Vector<string> SimilarityVector { get; set; }
    
    [Indexed(Sortable = true)]
    public string Type { get; set;} = null!;
    
    [Indexed(Sortable = true)]
    public string Language { get; set; } = string.Empty;
    
    [Indexed(Sortable = true)]
    public int Year { get; set; }
    
    public string Link { get; set; } = null!;
    
    [Searchable(Weight = 0.8)]
    public string[] Keywords { get; set; } = Array.Empty<string>();

    [Searchable(Weight = 0.7)] 
    public string Abstract { get; set; } = string.Empty;
    
    [Indexed(JsonPath = "$.Id")]
    [Searchable(JsonPath = "$.Name", Weight = 0.8, PhoneticMatcher = "dm:en")]
    public Author[] Authors { get; set; } = Array.Empty<Author>();
    
    [Indexed(JsonPath = "$.Id")]
    [Searchable(JsonPath = "$.Name", Weight = 0.8, PhoneticMatcher = "dm:en")]
    public Publisher? Publisher { get; set; }
    
    [Indexed(Sortable = true)]
    public int Views { get; set; } 
    
    [Indexed(JsonPath = "$.Id")]
    [IgnoreInResponse]
    public Filter[] Filters { get; set; } = Array.Empty<Filter>();
    
    [Indexed(JsonPath = "$.Id")]
    [Searchable(JsonPath = "$.Name", Weight = 0.8)]
    public Collection[] Collections { get; set; } = Array.Empty<Collection>();
    
    public override Publication UpdateSlug(IWordsService wordsService)
    {
        Slug = SlugFactory.Create(
            Title, Id.ToString(), IsoLanguageCode.Create(Language), wordsService);
        
        return this;
    }
    
    public Publication UpdateViews(int views = 1)
    {
        if (views < 0)
        {
            throw new ArgumentException("Views cannot be negative");
        }
        
        Views = views;
        return this;
    }
    
    public Publication UpdateVectors(IWordsService wordsService)
    {
        SimilarityVector = Vector.Of(wordsService
            .Transliterate(GetSimilarityValue())
            .RemoveSpecialChars());
        
        return this;
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

    private string GetSimilarityValue() =>
        Title + " " + 
        Abstract + " " +
        string.Join(" ", Keywords) + " " + 
        string.Join(" ", Collections.Select(c => c.Name));
}