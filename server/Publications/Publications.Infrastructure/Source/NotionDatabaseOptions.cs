using System.ComponentModel.DataAnnotations;

namespace Publications.Infrastructure.Source;

/// <summary>
/// Notion database ids encapsulated in one options record.
/// </summary>
public class NotionDatabaseOptions
{
    [Required] public string PublicationsDbId { get; init; } = null!;
    [Required] public string AuthorsDbId { get; init; } = null!;
    [Required] public string PublishersDbId { get; init; } = null!;
    [Required] public string CollectionsDbId { get; init; } = null!;
}