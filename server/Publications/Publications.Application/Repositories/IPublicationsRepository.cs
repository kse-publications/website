using Publications.Application.DTOs;
using Publications.Domain.Publications;

namespace Publications.Application.Repositories;

/// <summary>
/// Contract for a repository that manages <see cref="Publication"/>s.
/// </summary>
public interface IPublicationsRepository: IEntityRepository<Publication>
{
    Task<PaginatedCollection<Publication>> GetAllAsync(
        PaginationFilterDTO paginationDTO,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedCollection<Publication>> GetBySearchAsync(
        PaginationFilterSearchDTO paginationSearchDTO,
        CancellationToken cancellationToken = default);
}