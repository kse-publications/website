using Microsoft.EntityFrameworkCore;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using Publications.Application.Services;
using Publications.Domain.Collections;
using Publications.Domain.Filters;
using Publications.Domain.Publications;
using Publications.Infrastructure.Requests;
using Redis.OM;
using Redis.OM.Contracts;
using StackExchange.Redis;

namespace Publications.Infrastructure.Services;

public class DbConfigurationService : IDbConfigurationService
{
    private readonly IRedisConnectionProvider _redisConnectionProvider;
    private readonly IDatabase _db;
    private readonly RequestsHistoryDbContext _dbContext;

    public DbConfigurationService(
        IConnectionMultiplexer connectionMultiplexer,
        RequestsHistoryDbContext dbContext)
    {
        _redisConnectionProvider = new RedisConnectionProvider(connectionMultiplexer);
        _db = connectionMultiplexer.GetDatabase();
        _dbContext = dbContext;
    }

    public async Task ConfigureAsync()
    {
        await ConfigureRedisAsync();
        await ConfigureSqliteAsync();
    }

    private async Task ConfigureRedisAsync()
    {
        var indexes = (await _redisConnectionProvider.Connection
            .ExecuteAsync("FT._LIST")).ToArray();
        
        await CreateIndexesAsync(indexes,
            typeof(Publication),
            typeof(Collection),
            typeof(FilterGroup));
    }
    
    private async Task ConfigureSqliteAsync()
    {
        await _dbContext.Database.MigrateAsync();   
    }

    private async Task CreateIndexesAsync(RedisReply[] indexes, params Type[] types)
    {
        Dictionary<string, Type> indexesToCreate = types.ToDictionary(GetIndexName);
        SearchCommands ft = _db.FT();
        
        foreach (var currentIndex in indexesToCreate.Keys)
        {
            if (indexes.All(i => i != currentIndex))
            {
                await _redisConnectionProvider.Connection
                    .CreateIndexAsync(indexesToCreate[currentIndex]);
            }
        }
        
        foreach (var existingIndex in indexes)
        {
            if (!indexesToCreate.ContainsKey(existingIndex))
            {
                await ft.DropIndexAsync(existingIndex, dd: true);
            }
        }
    }

    private static string GetIndexName(Type type) => $"{type.Name.ToLower()}-idx";
}