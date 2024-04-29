using Publications.Application.Repositories;
using Publications.Domain.Shared;
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

    public async Task InsertOrUpdateAsync(
        IEnumerable<TEntity> entities, 
        CancellationToken cancellationToken = default)
    {
        await _collection.DeleteAsync(await _collection.ToListAsync());
        await _collection.InsertAsync(entities);
    }
    
    public async Task<TEntity?> GetByIdAsync(
        int resourceId, 
        CancellationToken cancellationToken = default)
    {
        return await _collection.FindByIdAsync(resourceId.ToString());
    }
    
    public async Task UpdateAsync(
        IEnumerable<TEntity> entities, 
        CancellationToken cancellationToken = default)
    {
        await _collection.UpdateAsync(entities);
    }
}