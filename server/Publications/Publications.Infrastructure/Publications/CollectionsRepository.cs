using Publications.Application.DTOs.Response;
using Publications.Application.Repositories;
using Publications.Domain.Collections;
using Publications.Infrastructure.Shared;
using Redis.OM;
using Redis.OM.Searching;
using StackExchange.Redis;

namespace Publications.Infrastructure.Publications;

public class CollectionsRepository: EntityRepository<Collection>, ICollectionsRepository
{
    private readonly IRedisCollection<Collection> _collections;
    public CollectionsRepository(IConnectionMultiplexer connection) 
        : base(connection)
    {
        RedisConnectionProvider provider = new(connection);
        _collections = provider.RedisCollection<Collection>();
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

    public async Task DeleteAsync(
        IEnumerable<int> ids, 
        CancellationToken cancellationToken = default)
    {
        await DeleteAsync(ids, GetCollectionKey, cancellationToken);
    }

    private static string GetCollectionKey(int id) => $"collection:{id}";
}