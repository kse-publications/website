using Publications.Application.DTOs.Response;

namespace Publications.Application.Repositories;

public interface IStatisticsRepository
{
    Task<OverallStats> GetOverallStatsAsync();
    Task<RecentStats> GetRecentStatsAsync();
    
    Task SetTotalPublicationsCountAsync(int count);
    Task IncrementTotalSearchesAsync(int searchesCount = 1);
    
    Task SetRecentViewsCountAsync(int count);
    Task SetTopRecentlyViewedPublicationsAsync(
        IEnumerable<PublicationSummary> publications);
}