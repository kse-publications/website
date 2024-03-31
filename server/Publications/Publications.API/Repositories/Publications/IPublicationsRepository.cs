using Publications.API.DTOs;
using Publications.API.Models;

namespace Publications.API.Repositories.Publications;

/// <summary>
/// Contract for a repository that manages <see cref="Publication"/>s.
/// </summary>
public interface IPublicationsRepository
{
    Task<PaginatedCollection<Publication>> GetAllAsync(
        PaginationFilterDTO paginationFilterDTO,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedCollection<Publication>> GetBySearchAsync(
        PaginationSearchDTO paginationSearchDTO,
        CancellationToken cancellationToken = default);
    
    Task<Publication?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default);
    
    Task InsertOrUpdateAsync(
        IEnumerable<Publication> publications,
        CancellationToken cancellationToken = default);
}