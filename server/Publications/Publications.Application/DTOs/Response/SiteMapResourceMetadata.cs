namespace Publications.Application.DTOs.Response;

public class SiteMapResourceMetadata
{
    public string Id { get; set; }
    public DateTime LastModifiedAt { get; set; }
    
    public SiteMapResourceMetadata(string id, DateTime lastModifiedAt)
    {
        Id = id;
        LastModifiedAt = lastModifiedAt;
    }
}