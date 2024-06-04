using Publications.Application.Repositories;
using Publications.Domain.Collections;
using Publications.Domain.Publications;
using Publications.Infrastructure.Shared;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace Publications.Infrastructure.Publications;

public class CollectionsRepository: EntityRepository<Collection>, ICollectionsRepository
{
    private readonly IRedisCollection<Collection> _collections;
    public CollectionsRepository(IRedisConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
        _collections = connectionProvider.RedisCollection<Collection>();
    }
    
    public async Task<IReadOnlyCollection<Collection>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return (await _collections.ToListAsync()).AsReadOnly();
    }
}