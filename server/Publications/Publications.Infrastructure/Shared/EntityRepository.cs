using System.Text.Json;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using Publications.Application.DTOs.Response;
using Publications.Domain.Shared;
using Redis.OM;
using Redis.OM.Aggregation;
using Redis.OM.Searching;
using StackExchange.Redis;

namespace Publications.Infrastructure.Shared;

public abstract class EntityRepository<TEntity> where TEntity : Entity
{
    private readonly IRedisCollection<TEntity> _collection;
    private readonly RedisAggregationSet<TEntity> _aggregationSet;
    private readonly IDatabase _db;
    private readonly JsonSerializerOptions _options;
        
    protected EntityRepository(
        IConnectionMultiplexer connection, 
        JsonSerializerOptions jsonOptions)
    {
        RedisConnectionProvider provider = new(connection);
        _collection = provider.RedisCollection<TEntity>();
        _aggregationSet = provider.AggregationSet<TEntity>();
        _db = connection.GetDatabase();
        _options = jsonOptions;
    }

    protected abstract string GetKey(int id);
    
    public async Task<TEntity?> GetByIdAsync(
        int resourceId, 
        CancellationToken cancellationToken = default)
    {
        JsonCommands json = _db.JSON();
        return await json.GetAsync<TEntity>(GetKey(resourceId), serializerOptions: _options);
    }

    public virtual async Task<IReadOnlyCollection<SiteMapResourceMetadata>> GetSiteMapMetadataAsync(
        CancellationToken cancellationToken = default)
    {
        return (await _aggregationSet
                .Load(e => e.RecordShell!.Slug)
                .Load(e => e.RecordShell!.LastModifiedAt)
                .ToListAsync())
            .Select(e => e.Hydrate())
            .Select(e => new SiteMapResourceMetadata(e.Slug, e.LastModifiedAt))
            .ToList()
            .AsReadOnly();
    }
    
    public virtual async Task InsertAsync(
        IEnumerable<TEntity> entities, 
        CancellationToken cancellationToken = default)
    {
        await _collection.InsertAsync(entities);
    }
    
    public virtual async Task UpdateAsync(
        IEnumerable<TEntity> entities, 
        CancellationToken cancellationToken = default)
    {
        await _collection.InsertAsync(entities);
    }

    public async Task DeleteAsync(
        IEnumerable<int> ids,
        Func<int, string> keyFunc,
        CancellationToken cancellationToken)
    {
        JsonCommands json = _db.JSON();
        foreach (var id in ids)
        {
            await json.DelAsync(keyFunc(id));
        }
    }
}