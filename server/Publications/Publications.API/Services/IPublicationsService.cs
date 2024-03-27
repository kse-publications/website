using Publications.API.DTOs;
using Publications.API.Models;
using Publications.API.Repositories;

namespace Publications.API.Services;

public interface IPublicationsService
{
    Task<PaginatedCollection<Publication>> GetAllAsync(
        PaginationDTO paginationDto,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedCollection<Publication>> GetByFullTextSearchAsync(
        PaginationSearchDTO paginationSearchDto,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedCollection<Publication>> GetByAutoCompleteAsync(
        PaginationSearchDTO paginationSearchDto,
        CancellationToken cancellationToken = default);
    
    Task<Publication?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default);
    
    Task InsertOrUpdateAsync(
        IEnumerable<Publication> publications,
        CancellationToken cancellationToken = default);
}