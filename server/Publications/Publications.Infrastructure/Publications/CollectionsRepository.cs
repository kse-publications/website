using System.Text.Json;
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
    public CollectionsRepository(
        IConnectionMultiplexer connection,
        JsonSerializerOptions jsonOptions) 
        : base(connection, jsonOptions)
    {
        RedisConnectionProvider provider = new(connection);
        _collections = provider.RedisCollection<Collection>();
    }

    protected override string GetKey(int id) => $"collection:{id}";

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
            PublicationsIds = Collection.ParsePublicationIds(values.PublicationsIds)
        }).ToList().AsReadOnly();
    }

    public async Task DeleteAsync(
        IEnumerable<int> ids, 
        CancellationToken cancellationToken = default)
    {
        await DeleteAsync(ids, GetKey, cancellationToken);
    }
}