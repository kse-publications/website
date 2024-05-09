using Publications.Application.Repositories;
using Publications.Domain.Shared;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace Publications.Infrastructure.Shared;

public class EntityRepository<TEntity> : IEntityRepository<TEntity> 
    where TEntity : Entity<TEntity>
{
    private readonly IRedisCollection<TEntity> _collection;

    public EntityRepository(IRedisConnectionProvider connectionProvider)
    {
        _collection = connectionProvider.RedisCollection<TEntity>();
    }
    
    public async Task<IReadOnlyCollection<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return (await _collection.ToListAsync()).AsReadOnly();
    }
    
    public async Task<TEntity?> GetByIdAsync(
        int resourceId, 
        CancellationToken cancellationToken = default)
    {
        return await _collection.FindByIdAsync(resourceId.ToString());
    }
    
    public virtual async Task InsertOrUpdateAsync(
        IEnumerable<TEntity> entities, 
        CancellationToken cancellationToken = default)
    {
        await _collection.InsertAsync(entities);
    }
    
    /// <summary>
    /// Deletes entities that were last synchronized before the given date.
    /// </summary>
    public virtual async Task SynchronizeAsync(
        DateTime lastSyncDateTime, 
        CancellationToken cancellationToken = default)
    {
        var entitiesToDelete = await _collection
            .Where(e => e.SynchronizedAt < lastSyncDateTime)
            .ToListAsync();
        
        await _collection.DeleteAsync(entitiesToDelete);
    }
}