using Publications.Application.DTOs;

namespace Publications.Application.Statistics;

public interface IStatisticsRepository
{
    Task<OverallStats> GetOverallStatsAsync();
    Task<RecentStats> GetRecentStatsAsync();
    
    Task SetTotalPublicationsCountAsync(int count);
    Task IncrementTotalSearchesAsync(int searchesCount = 1);
    
    Task SetRecentViewsCountAsync(int count);
    Task SetTopRecentlyViewedPublicationsAsync(PublicationSummary[] publications);
}