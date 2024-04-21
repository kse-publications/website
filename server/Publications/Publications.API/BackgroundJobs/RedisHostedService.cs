using Publications.API.Models;
using Redis.OM;
using Redis.OM.Contracts;

namespace Publications.API.BackgroundJobs;

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

        if (indexes.All(i => i != "publication-idx"))
        {
            await _redisConnectionProvider.Connection.CreateIndexAsync(typeof(Publication));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}