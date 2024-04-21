namespace Publications.API.DTOs;

/// <summary>
/// Notion database ids encapsulated in one options record.
/// </summary>
public class NotionDatabaseOptions
{
    public string PublicationsDbId { get; init; } = null!;
    public string AuthorsDbId { get; init; } = null!;
    public string PublishersDbId { get; init; } = null!;
}