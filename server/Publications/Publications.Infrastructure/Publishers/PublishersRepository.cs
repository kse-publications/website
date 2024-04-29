using Publications.Application.Repositories;
using Publications.Domain.Publishers;
using Publications.Infrastructure.Shared;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace Publications.Infrastructure.Publishers;

public class PublishersRepository: EntityRepository<Publisher>, IPublishersRepository
{
    private readonly IRedisCollection<Publisher> _publishers;

    public PublishersRepository(IRedisConnectionProvider provider)
        : base(provider)
    {
        _publishers = provider.RedisCollection<Publisher>();
    }
}