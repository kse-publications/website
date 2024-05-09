using Publications.Domain.Authors;
using Publications.Domain.Publishers;
using Publications.Domain.Shared;
using Publications.Domain.Shared.ValueObjects;
using Redis.OM.Modeling;

namespace Publications.Domain.Publications;

/// <summary>
/// Aggregate root that represents a publication.
/// </summary>
[Document(IndexName = "publication-idx", StorageType = StorageType.Json, Prefixes = ["publication"])]
public class Publication: Entity<Publication>
{
    [Searchable(Weight = 1.0)]
    public string Title { get; set; } = null!;
    
    [Indexed(Sortable = true)]
    public string Type { get; set;} = null!;
    
    [Indexed]
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
    
    public override Publication UpdateSlug(IWordsService wordsService)
    {
        Slug = SlugService.Create(
            Title, Id.ToString(), IsoLanguageCode.Create(Language), wordsService);
        
        return this;
    }
}