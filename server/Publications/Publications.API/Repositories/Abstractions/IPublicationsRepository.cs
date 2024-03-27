using Publications.API.DTOs;
using Publications.API.Models;

namespace Publications.API.Repositories.Abstractions;

/// <summary>
/// Contract for a repository that manages <see cref="Publication"/>s.
/// </summary>
public interface IPublicationsRepository
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