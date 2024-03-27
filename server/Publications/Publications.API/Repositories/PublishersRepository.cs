using Publications.API.Models;
using Publications.API.Repositories.Abstractions;
using Redis.OM;
using Redis.OM.Searching;

namespace Publications.API.Repositories;

public class PublishersRepository: IPublishersRepository
{
    private readonly IRedisCollection<Publisher> _publishers;

    public PublishersRepository(RedisConnectionProvider provider)
    {
        _publishers = provider.RedisCollection<Publisher>();
    }

    public async Task InsertOrUpdateAsync(
        IEnumerable<Publisher> publishers, CancellationToken cancellationToken = default)
    {
        await _publishers.InsertAsync(publishers);
    }
}