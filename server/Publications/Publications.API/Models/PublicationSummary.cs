namespace Publications.API.Models;

/// <summary>
/// A DTO that represents a publication summary.
/// Only part of the <see cref="Publication"/>'s properties are included.
/// </summary>
public class PublicationSummary
{ 
    public string Slug { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Type { get; set; } = string.Empty;
    public int Year { get; set; }
    public string[] Authors { get; set; } = Array.Empty<string>();
    public string Publisher { get; set; } = string.Empty;
    
    public static PublicationSummary FromPublication(Publication publication)
    {
        return new PublicationSummary
        {
            Slug = publication.Slug,
            Title = publication.Title,
            Type = publication.Type,
            Year = publication.Year,
            Authors = publication.Authors.Select(a => a.Name).ToArray(),
            Publisher = publication.Publisher?.Name ?? string.Empty
        };
    }
}