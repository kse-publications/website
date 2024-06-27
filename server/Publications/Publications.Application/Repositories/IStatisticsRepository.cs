using Publications.Application.DTOs.Response;

namespace Publications.Application.Repositories;

public interface IStatisticsRepository
{
    Task<OverallStats> GetOverallStatsAsync(CancellationToken cancellationToken = default);
    Task<RecentStats> GetRecentStatsAsync(CancellationToken cancellationToken = default);
    
    Task SetTotalPublicationsCountAsync(int count);
    Task IncrementTotalSearchesAsync(int searchesCount = 1);
    Task SetTotalViewsCountAsync(int count);
    
    Task SetRecentViewsCountAsync(int count);
    Task SetTopRecentlyViewedPublicationsAsync(
        IEnumerable<PublicationSummary> publications);
}