using Publications.API.DTOs;
using Publications.API.Models;
using Publications.API.Repositories;

namespace Publications.API.Services;

public interface IPublicationsService
{
    Task<PaginatedCollection<PublicationSummary>> GetAllAsync(
        PaginationDTO paginationDto,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        PaginationSearchDTO paginationSearchDto,
        CancellationToken cancellationToken = default);
    
    Task<Publication?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default);
    
    Task InsertOrUpdateAsync(
        IEnumerable<Publication> publications,
        CancellationToken cancellationToken = default);
}