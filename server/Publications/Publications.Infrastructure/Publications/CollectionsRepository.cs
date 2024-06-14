using Publications.Application.DTOs.Response;
using Publications.Application.Repositories;
using Publications.Domain.Collections;
using Publications.Infrastructure.Shared;
using Redis.OM;
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

    public async Task<IReadOnlyCollection<SyncCollectionMetadata>> GetAllSyncMetadataAsync(
        CancellationToken cancellationToken = default)
    {
        return (await _collections.Select(c => new
        {
            c.Id,
            c.LastModifiedAt,
            c.PublicationsIds
        }).ToListAsync()).Select(values => new SyncCollectionMetadata
        {
            Id = values.Id,
            LastSynchronizedAt = values.LastModifiedAt,
            PublicationsIds = Collection.GetPublicationIds(values.PublicationsIds)
        }).ToList().AsReadOnly();
    }
}