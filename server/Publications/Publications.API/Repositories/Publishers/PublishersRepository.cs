using Publications.API.Models;
using Redis.OM;
using Redis.OM.Searching;

namespace Publications.API.Repositories.Publishers;

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