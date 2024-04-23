using Publications.API.DTOs;
using Publications.API.Models;
using Publications.API.Repositories;
using Publications.API.Repositories.Shared;

namespace Publications.API.Services;

public interface IPublicationsService
{
    Task<PaginatedCollection<PublicationSummary>> GetAllAsync(
        PaginationFilterDTO paginationDTO,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        PaginationFilterSearchDTO paginationSearchDTO,
        CancellationToken cancellationToken = default);
    
    Task<Publication?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default);
}