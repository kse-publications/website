namespace Publications.API.Models;

public class Request
{
    public int Id { get; set; }
    public string SessionId { get; init; }
    public string ResourceName { get; init;}
    public int ResourceId { get; init; }
    public DateTime RequestedAt { get; init;}
    
    // EF Core constructor
    private Request()
    {
    }

    public Request(
        string sessionId,
        string resourceName,
        int resourceId)
    {
        SessionId = sessionId;
        ResourceName = resourceName;
        ResourceId = resourceId;
        RequestedAt = DateTime.UtcNow;
    }
}