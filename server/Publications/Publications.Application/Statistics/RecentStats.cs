using Publications.Application.DTOs;

namespace Publications.Application.Statistics;

public class RecentStats
{
    public int RecentViewsCount { get; set; }
    public PublicationSummary[] TopRecentlyViewedPublications { get; set; } = [];
}