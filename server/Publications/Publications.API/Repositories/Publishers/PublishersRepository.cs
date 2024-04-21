using Publications.API.Models;
using Publications.API.Repositories.Shared;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace Publications.API.Repositories.Publishers;

public class PublishersRepository: EntityRepository<Publisher>, IPublishersRepository
{
    private readonly IRedisCollection<Publisher> _publishers;

    public PublishersRepository(IRedisConnectionProvider provider)
        : base(provider)
    {
        _publishers = provider.RedisCollection<Publisher>();
    }
}