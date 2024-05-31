using NRedisStack;
using NRedisStack.RedisStackCommands;
using Publications.Application.Services;
using Publications.Domain.Publications;
using Redis.OM;
using Redis.OM.Contracts;
using StackExchange.Redis;

namespace Publications.Infrastructure.Services;

public class RedisConfigurationService: IDbConfigurationService
{
    private readonly IRedisConnectionProvider _redisConnectionProvider;
    private readonly SearchCommands _ft;

    public RedisConfigurationService(
        IRedisConnectionProvider redisConnectionProvider, 
        IConnectionMultiplexer connectionMultiplexer)
    {
        _redisConnectionProvider = redisConnectionProvider;
        _ft = connectionMultiplexer.GetDatabase().FT();
    }

    public async Task ConfigureAsync()
    {
        var indexes = (await _redisConnectionProvider.Connection
            .ExecuteAsync("FT._LIST")).ToArray();
        
        await CreateIndexesAsync(indexes, 
            typeof(Publication),
            typeof(Collection), 
            typeof(FilterGroup));
    }

    private async Task CreateIndexesAsync(RedisReply[] indexes, params Type[] types)
    {
        Dictionary<string, Type> indexesToCreate = types.ToDictionary(GetIndexName);
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
                await _ft.DropIndexAsync(existingIndex, dd: true);
            }
        }
    }

    private static string GetIndexName(Type type) => $"{type.Name.ToLower()}-idx";
}