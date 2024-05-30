using Publications.Domain.Publications;

namespace Publications.Application.Repositories;

public interface ICollectionsRepository : IEntityRepository<Collection>
{
    Task<IReadOnlyCollection<Collection>> GetAllAsync(
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<SiteMapResourceMetadata>> GetAllSiteMapMetadataAsync(
        CancellationToken cancellationToken = default);
}