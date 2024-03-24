using Publications.API.Models;
using Redis.OM;

namespace Publications.API.BackgroundJobs;

public class RedisHostedService: IHostedService
{
    private readonly RedisConnectionProvider _redisConnectionProvider;

    public RedisHostedService(RedisConnectionProvider redisConnectionProvider)
    {
        _redisConnectionProvider = redisConnectionProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var indexes = (await _redisConnectionProvider.Connection
            .ExecuteAsync("FT._LIST")).ToArray();

        if (indexes.All(i => i != $"{nameof(Publication).ToLower()}-idx"))
        {
            await _redisConnectionProvider.Connection.CreateIndexAsync(typeof(Publication));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}