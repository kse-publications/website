using Publications.Application.Services;
using Publications.Domain.Authors;
using Publications.Domain.Publications;
using Publications.Domain.Publishers;
using Redis.OM;
using Redis.OM.Contracts;

namespace Publications.Infrastructure.Services;

public class RedisConfigurationService: IDbConfigurationService
{
    private readonly IRedisConnectionProvider _redisConnectionProvider;

    public RedisConfigurationService(IRedisConnectionProvider redisConnectionProvider)
    {
        _redisConnectionProvider = redisConnectionProvider;
    }

    public async Task ConfigureAsync()
    {
        await ClearAllAsync();

        await CreateIndexAsync<Publication>();
        await CreateIndexAsync<Publisher>();
        await CreateIndexAsync<Author>();
        await CreateIndexAsync<FilterGroup>();
    }

    private async Task CreateIndexAsync<T>()
    {
        await _redisConnectionProvider.Connection.CreateIndexAsync(typeof(T));
    }
    
    private async Task ClearAllAsync()
    {
        await _redisConnectionProvider.Connection.ExecuteAsync("FLUSHDB");
    }
}