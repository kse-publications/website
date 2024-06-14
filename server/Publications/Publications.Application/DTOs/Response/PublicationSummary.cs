using Publications.Domain.Publications;

namespace Publications.Application.DTOs.Response;

/// <summary>
/// A DTO that represents a publication summary.
/// Only part of the <see cref="Publication"/>'s properties are included.
/// </summary>
public class PublicationSummary
{ 
    public string Slug { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string Type { get; init; } = string.Empty;
    public int Year { get; init; }
    public string[] Authors { get; init; } = Array.Empty<string>();
    public string Publisher { get; init; } = string.Empty;
    
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