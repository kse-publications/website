namespace Publications.API.Models;

/// <summary>
/// A DTO that represents a publication summary.
/// Only part of the <see cref="Publication"/>'s properties are included.
/// </summary>
public class PublicationSummary
{ 
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Link { get; set; } = null!;
    public string[] Keywords { get; set; } = Array.Empty<string>();
}