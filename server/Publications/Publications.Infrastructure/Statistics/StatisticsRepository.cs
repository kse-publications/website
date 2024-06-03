using System.Text.Json;
using Publications.Application.DTOs;
using Publications.Application.Statistics;
using StackExchange.Redis;

namespace Publications.Infrastructure.Statistics;

public class StatisticsRepository: IStatisticsRepository
{
    private readonly IDatabase _db;

    public StatisticsRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        _db = connectionMultiplexer.GetDatabase();
    }
    
    public async Task<OverallStats> GetOverallStatsAsync()
    {
        var totalPublicationsCount = await _db
            .StringGetAsync(nameof(OverallStats.TotalPublicationsCount));
        var totalSearchesCount = await _db
            .StringGetAsync(nameof(OverallStats.TotalSearchesCount));
        
        return new OverallStats
        {
            TotalPublicationsCount = (int)totalPublicationsCount,
            TotalSearchesCount = (long)totalSearchesCount
        };
    }

    public async Task<RecentStats> GetRecentStatsAsync()
    {
        var recentViewsCount = await _db
            .StringGetAsync(nameof(RecentStats.RecentViewsCount));
        var topRecentlyViewedPublications = await _db
            .StringGetAsync(nameof(RecentStats.TopRecentlyViewedPublications));
        
        return new RecentStats
        {
            RecentViewsCount = (int)recentViewsCount,
            TopRecentlyViewedPublications = JsonSerializer
                .Deserialize<PublicationSummary[]>((string?)topRecentlyViewedPublications ?? "[]")!
        };
    }

    public async Task SetTotalPublicationsCountAsync(int count)
    {
        await _db.StringSetAsync(nameof(OverallStats.TotalPublicationsCount), count);
    }

    public async Task IncrementTotalSearchesAsync(int searchesCount = 1)
    {
        await _db.StringIncrementAsync(nameof(OverallStats.TotalSearchesCount), searchesCount);
    }
    
    public async Task SetRecentViewsCountAsync(int count)
    {
        await _db.StringSetAsync(nameof(RecentStats.RecentViewsCount), count);
    }
    
    public async Task SetTopRecentlyViewedPublicationsAsync(PublicationSummary[] publications)
    {
        string serializedPublications = JsonSerializer.Serialize(publications);
        await _db.StringSetAsync(nameof(RecentStats.TopRecentlyViewedPublications), serializedPublications);
    }
}