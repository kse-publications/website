namespace Publications.API.Models;

/// <summary>
/// A DTO that represents a publication summary.
/// Only part of the <see cref="Publication"/>'s properties are included.
/// </summary>
public class PublicationSummary
{ 
    public string Slug { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Link { get; set; } = null!;
    public string[] Keywords { get; set; } = Array.Empty<string>();
    
    public static PublicationSummary FromPublication(Publication publication)
    {
        return new PublicationSummary
        {
            Slug = publication.Slug,
            Title = publication.Title,
            Link = publication.Link,
            Keywords = publication.Keywords
        };
    }
}