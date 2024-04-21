
using Publications.API.Models;

namespace Publications.API.Repositories.Shared;

public interface IEntityRepository<TEntity>
    where TEntity : Entity<TEntity>
{
    Task<IReadOnlyCollection<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default);
    
    Task InsertOrUpdateAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(
        int resourceId,
        CancellationToken cancellationToken = default);
    
    Task UpdateAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);
}