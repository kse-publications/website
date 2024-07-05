using Publications.Application.DTOs.Response;
using Publications.Domain.Shared;

namespace Publications.Application.Repositories;

public interface IEntityRepository<TEntity> where TEntity : Entity
{
    Task<TEntity?> GetByIdAsync(
        int resourceId,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<SiteMapResourceMetadata>> GetSiteMapMetadataAsync(
        CancellationToken cancellationToken = default);
    
    Task InsertAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);
    
    Task UpdateAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);
    
    Task DeleteAsync(
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default);
}