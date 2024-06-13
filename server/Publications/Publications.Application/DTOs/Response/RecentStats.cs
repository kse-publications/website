namespace Publications.Application.DTOs.Response;

public class RecentStats
{
    public int RecentViewsCount { get; set; }
    public PublicationSummary[] TopRecentlyViewedPublications { get; set; } = [];
}