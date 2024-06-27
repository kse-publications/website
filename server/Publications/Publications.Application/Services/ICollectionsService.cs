using Publications.Application.DTOs.Request;
using Publications.Application.DTOs.Response;
using Publications.Domain.Collections;

namespace Publications.Application.Services;

public interface ICollectionsService
{
    Task<IReadOnlyCollection<Collection>> GetAllAsync(
        CancellationToken cancellationToken = default);
    
    Task<CollectionData?> GetByIdAsync(
        int id, PaginationDTO paginationDTO, 
        CancellationToken cancellationToken = default);
}