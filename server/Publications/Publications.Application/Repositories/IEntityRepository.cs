using Publications.Application.DTOs.Response;
using Publications.Domain.Shared;

namespace Publications.Application.Repositories;

public interface IEntityRepository<TEntity> where TEntity : Entity
{
    Task<TEntity?> GetByIdAsync(
        int resourceId,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<SiteMapResourceMetadata>> GetSiteMapMetadataAsync(
        CancellationToken cancellationToken = default);
    
    Task InsertOrUpdateAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);
    
    Task DeleteAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);
    
    Task SynchronizeAsync(
        DateTime lastSyncDateTime,
        CancellationToken cancellationToken = default);
}