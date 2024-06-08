using Publications.Application.DTOs.Response;
using Publications.Application.Repositories;
using Publications.Domain.Shared;
using Redis.OM;
using Redis.OM.Aggregation;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace Publications.Infrastructure.Shared;

public class EntityRepository<TEntity> : IEntityRepository<TEntity> 
    where TEntity : Entity
{
    private readonly IRedisCollection<TEntity> _collection;
    private readonly RedisAggregationSet<TEntity> _aggregationSet;

    protected EntityRepository(IRedisConnectionProvider connectionProvider)
    {
        _collection = connectionProvider.RedisCollection<TEntity>();
        _aggregationSet = connectionProvider.AggregationSet<TEntity>();
    }
    
    public async Task<TEntity?> GetByIdAsync(
        int resourceId, 
        CancellationToken cancellationToken = default)
    {
        return await _collection.FindByIdAsync(resourceId.ToString());
    }

    public virtual async Task<IReadOnlyCollection<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return (await _collection.ToListAsync()).AsReadOnly();
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
    
    public virtual async Task InsertOrUpdateAsync(
        IEnumerable<TEntity> entities, 
        CancellationToken cancellationToken = default)
    {
        await _collection.InsertAsync(entities);
    }
    
    public virtual async Task DeleteAsync(
        IEnumerable<TEntity> entities, 
        CancellationToken cancellationToken = default)
    {
        await _collection.DeleteAsync(entities);
    }
    
    /// <summary>
    /// Deletes entities that were last synchronized before the given date.
    /// </summary>
    public virtual async Task SynchronizeAsync(
        DateTime lastSyncDateTime, 
        CancellationToken cancellationToken = default)
    {
        var entitiesToDelete = await _collection
            .Where(e => e.LastSynchronizedAt < lastSyncDateTime)
            .ToListAsync();
        
        await _collection.DeleteAsync(entitiesToDelete);
    }
}