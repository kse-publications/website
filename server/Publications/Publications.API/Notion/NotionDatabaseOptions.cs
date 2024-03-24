namespace Publications.API.Notion;

public record NotionDatabaseOptions
{
    public string PublicationsDbId { get; init; } = null!;
    public string AuthorsDbId { get; init; } = null!;
    public string PublishersDbId { get; init; } = null!;
}