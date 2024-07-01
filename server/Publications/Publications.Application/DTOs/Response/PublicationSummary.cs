using System.Text.Json.Serialization;
using Publications.Domain.Publications;

namespace Publications.Application.DTOs.Response;

/// <summary>
/// A DTO that represents a publication summary.
/// Only part of the <see cref="Publication"/>'s properties are included.
/// </summary>
public class PublicationSummary
{ 
    public string Slug { get; init; }
    public string Title { get; init; }
    public string Type { get; init; }
    public int Year { get; init; }
    public string[] Authors { get; init; }
    public string Publisher { get; init; }

    [JsonConstructor]
    public PublicationSummary(
        string slug,
        string title,
        string type,
        int year,
        string[] authors,
        string publisher)
    {
        Slug = slug;
        Title = title;
        Type = type;
        Year = year;
        Authors = authors;
        Publisher = publisher;
    }

    public PublicationSummary(Publication publication)
    {
        Slug = publication.Slug;
        Title = publication.Title;
        Type = publication.Type;
        Year = publication.Year;
        Authors = publication.Authors.Select(a => a.Name).ToArray();
        Publisher = publication.Publisher?.Name ?? string.Empty;
    }
}