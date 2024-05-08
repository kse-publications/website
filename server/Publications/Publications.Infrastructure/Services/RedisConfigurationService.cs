using Publications.Application.Services;
using Publications.Domain.Authors;
using Publications.Domain.Filters;
using Publications.Domain.Publications;
using Publications.Domain.Publishers;
using Redis.OM;
using Redis.OM.Contracts;

namespace Publications.Infrastructure.Shared;

public class RedisConfigurationService: IDbConfigurationService
{
    private readonly IRedisConnectionProvider _redisConnectionProvider;

    public RedisConfigurationService(IRedisConnectionProvider redisConnectionProvider)
    {
        _redisConnectionProvider = redisConnectionProvider;
    }

    public async Task ConfigureAsync()
    {
        var indexes = (await _redisConnectionProvider.Connection
            .ExecuteAsync("FT._LIST")).ToArray();

        await CreateIndexAsync<Publication>(indexes);
        await CreateIndexAsync<Publisher>(indexes);
        await CreateIndexAsync<Author>(indexes);
        await CreateIndexAsync<FilterGroup>(indexes);
    }

    private async Task CreateIndexAsync<T>(RedisReply[] indexes)
    {
        if (indexes.All(i => i != GetIndexName<T>()))
        {
            await _redisConnectionProvider.Connection.CreateIndexAsync(typeof(T));
        }
    }
    
    private static string GetIndexName<T>() => $"{typeof(T).Name.ToLower()}-idx";
}