
namespace Publications.Application.DTOs.Response;

public class SyncEntityMetadata
{
    public int Id { get; set; }
    public DateTime LastSynchronizedAt { get; set; }
}

public class SyncCollectionMetadata : SyncEntityMetadata
{
    public int[] PublicationsIds { get; set; } = Array.Empty<int>();
}