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
        throw new NotImplementedException();
    }

    public async Task SetTotalPublicationsCountAsync(int count)
    {
        await _db.StringSetAsync(nameof(OverallStats.TotalPublicationsCount), count);
    }

    public async Task IncrementTotalSearchesAsync(int searchesCount = 1)
    {
        await _db.StringIncrementAsync(nameof(OverallStats.TotalSearchesCount), searchesCount);
    }
}