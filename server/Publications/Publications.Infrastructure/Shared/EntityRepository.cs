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
        await _collection.UpdateAsync(entities);
    }
    
    public virtual async Task DeleteAsync(
        IEnumerable<TEntity> entities, 
        CancellationToken cancellationToken = default)
    {
        await _collection.DeleteAsync(entities);
    }
    
}