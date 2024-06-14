using Publications.Application.DTOs.Response;
using Publications.Domain.Collections;

namespace Publications.Application.Repositories;

public interface ICollectionsRepository : IEntityRepository<Collection>
{
    Task<IReadOnlyCollection<Collection>> GetAllAsync(
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<SyncCollectionMetadata>> GetAllSyncMetadataAsync(
        CancellationToken cancellationToken = default);
}