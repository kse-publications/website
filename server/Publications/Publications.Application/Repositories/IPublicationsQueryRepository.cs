using Publications.Application.DTOs;
using Publications.Domain.Publications;

namespace Publications.Application.Repositories;

/// <summary>
/// Contract for a repository that manages <see cref="Publication"/>s.
/// </summary>
public interface IPublicationsQueryRepository
{
    Task<PaginatedCollection<PublicationSummary>> GetAllAsync(
        PaginationFilterDTO paginationDTO,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        PaginationFilterSearchDTO paginationSearchDTO,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns the number of publications that match each filter, if it would be applied.
    /// Basically a group by filter id query. 
    /// </summary>
    Task<Dictionary<string, int>> GetFiltersCountAsync(
        PaginationFilterSearchDtoV2 filterSearchDTO,
        CancellationToken cancellationToken = default);
}