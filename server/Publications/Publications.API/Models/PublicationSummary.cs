namespace Publications.API.Models;

public class PublicationSummary
{ 
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Link { get; set; } = null!;
    public string[] Keywords { get; set; } = Array.Empty<string>();
}