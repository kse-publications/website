namespace Publications.Application;

public class SiteMapResourceMetadata
{
    public int Id { get; set; }
    public DateTime LastModifiedAt { get; set; }
    
    public SiteMapResourceMetadata(int id, DateTime lastModifiedAt)
    {
        Id = id;
        LastModifiedAt = lastModifiedAt;
    }
}