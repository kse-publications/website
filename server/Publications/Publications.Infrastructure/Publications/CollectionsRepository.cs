using Publications.Application.Repositories;
using Publications.Domain.Collections;
using Publications.Domain.Publications;
using Publications.Infrastructure.Shared;
using Redis.OM.Contracts;

namespace Publications.Infrastructure.Publications;

public class CollectionsRepository: EntityRepository<Collection>, ICollectionsRepository
{
    public CollectionsRepository(IRedisConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
    }
}