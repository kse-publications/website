using Microsoft.Extensions.Hosting;
using Publications.Domain.Authors;
using Publications.Domain.Filters;
using Publications.Domain.Publications;
using Publications.Domain.Publishers;
using Redis.OM;
using Redis.OM.Contracts;

namespace Publications.BackgroundJobs;

public class RedisHostedService: IHostedService
{
    private readonly IRedisConnectionProvider _redisConnectionProvider;

    public RedisHostedService(IRedisConnectionProvider redisConnectionProvider)
    {
        _redisConnectionProvider = redisConnectionProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
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

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}