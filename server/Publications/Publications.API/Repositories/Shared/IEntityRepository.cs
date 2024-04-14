using Publications.API.Models;

namespace Publications.API.Repositories.Shared;

public interface IEntityRepository<TEntity> 
    where TEntity : Entity<TEntity>
{
    Task InsertOrUpdateAsync(
        IEnumerable<TEntity> entities, 
        CancellationToken cancellationToken = default);
    
}