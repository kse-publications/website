using Publications.Application.DTOs.Response;
using System.Text.Json;
using Publications.Application.Repositories;
using StackExchange.Redis;

namespace Publications.Infrastructure.Statistics;

public class StatisticsRepository: IStatisticsRepository
{
    private readonly IDatabase _db;

    public StatisticsRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        _db = connectionMultiplexer.GetDatabase();
    }
    
    public async Task<OverallStats> GetOverallStatsAsync(CancellationToken cancellationToken = default)
    {
        return new OverallStats(
            totalPublicationsCount: (int)await _db.StringGetAsync(nameof(OverallStats.TotalPublicationsCount)),
            totalSearchesCount: (long)await _db.StringGetAsync(nameof(OverallStats.TotalSearchesCount)),
            totalViewsCount: (int)await _db.StringGetAsync(nameof(OverallStats.TotalViewsCount)));
    }

    public async Task<RecentStats> GetRecentStatsAsync(CancellationToken cancellationToken = default)
    {
        var recentViewsCount = await _db
            .StringGetAsync(nameof(RecentStats.RecentViewsCount));
        var topRecentlyViewedPublications = (string?)await _db
            .StringGetAsync(nameof(RecentStats.TopRecentlyViewedPublications)) ?? "[]";

        return new RecentStats(
            recentViewsCount: (int)recentViewsCount,
            topRecentlyViewedPublications: JsonSerializer
                .Deserialize<PublicationSummary[]>(topRecentlyViewedPublications)!);
    }

    public async Task SetTotalPublicationsCountAsync(int count)
    {
        await _db.StringSetAsync(nameof(OverallStats.TotalPublicationsCount), count);
    }

    public async Task IncrementTotalSearchesAsync(int searchesCount = 1)
    {
        await _db.StringIncrementAsync(nameof(OverallStats.TotalSearchesCount), searchesCount);
    }
    
    public async Task SetTotalViewsCountAsync(int count)
    {
        await _db.StringSetAsync(nameof(OverallStats.TotalViewsCount), count);
    }
    
    public async Task SetRecentViewsCountAsync(int count)
    {
        await _db.StringSetAsync(nameof(RecentStats.RecentViewsCount), count);
    }
    
    public async Task SetTopRecentlyViewedPublicationsAsync(
        IEnumerable<PublicationSummary> publications)
    {
        string serializedPublications = JsonSerializer.Serialize(publications);
        await _db.StringSetAsync(nameof(RecentStats.TopRecentlyViewedPublications), serializedPublications);
    }
}