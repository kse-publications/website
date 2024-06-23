namespace Publications.Application.DTOs.Response;

public class RecentStats
{
    public int RecentViewsCount { get; init; }
    public PublicationSummary[] TopRecentlyViewedPublications { get; init; }
    
    public RecentStats(
        int recentViewsCount,
        PublicationSummary[] topRecentlyViewedPublications)
    {
        RecentViewsCount = recentViewsCount;
        TopRecentlyViewedPublications = topRecentlyViewedPublications;
    }
}