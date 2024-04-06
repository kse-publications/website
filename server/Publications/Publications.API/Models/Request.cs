namespace Publications.API.Models;

public class Request
{
    public string IpAddress { get; set; }
    public string UserAgent { get; set; } 
    
    public string ResourceName { get; set; }
    public string ResourceId { get; set; }
    
    public DateTime RequestedAt { get; set; }

    public Request(
        string ipAddress, 
        string userAgent,
        string resourceName,
        string resourceId)
    {
        IpAddress = ipAddress;
        UserAgent = userAgent;
        ResourceName = resourceName;
        ResourceId = resourceId;
        RequestedAt = DateTime.UtcNow;
    }
}