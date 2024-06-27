using Publications.Application.DTOs.Request;
using Publications.Application.DTOs.Response;
using Publications.Application.Repositories;
using Publications.Application.Services;
using Publications.Domain.Collections;

namespace Publications.Infrastructure.Services;

public class CollectionsService: ICollectionsService
{
    private readonly ICollectionsRepository _collectionsRepository;
    private readonly IPublicationsRepository _publicationsRepository;

    public CollectionsService(
        ICollectionsRepository collectionsRepository, 
        IPublicationsRepository publicationsRepository)
    {
        _collectionsRepository = collectionsRepository;
        _publicationsRepository = publicationsRepository;
    }

    public Task<IReadOnlyCollection<Collection>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _collectionsRepository.GetAllAsync(cancellationToken);
    }

    public async Task<CollectionData?> GetByIdAsync(int id, PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
    {
        Task<Collection?> collectionTask = _collectionsRepository.GetByIdAsync(id, cancellationToken);
        Task<PaginatedCollection<PublicationSummary>> publicationsTask = _publicationsRepository
            .GetFromCollectionAsync(id, paginationDTO, cancellationToken);
        
        await Task.WhenAll(collectionTask, publicationsTask);
        
        Collection? collection = await collectionTask;
        if (collection is null)
            return null;
        
        return new CollectionData(collection, await publicationsTask);
    }
}