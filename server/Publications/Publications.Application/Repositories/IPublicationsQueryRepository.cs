using Publications.Application.DTOs;
using Publications.Domain.Publications;

namespace Publications.Application.Repositories;

/// <summary>
/// Contract for a repository that manages <see cref="Publication"/>s.
/// </summary>
public interface IPublicationsQueryRepository
{
    Task<PaginatedCollection<PublicationSummary>> GetAllAsync(
        FilterDTO filterDTO,
        PaginationDTO paginationDTO,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        FilterDTO filterDTO,
        PaginationDTO paginationDTO,
        SearchDTO searchDTO,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns the number of publications that match each filter, if it would be applied.
    /// Basically a group by filter id query. 
    /// </summary>
    Task<Dictionary<string, int>> GetFiltersCountAsync(
        FilterDTOV2 filterDtov2,
        PaginationDTO paginationDTO,
        SearchDTO searchDTO,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedCollection<PublicationSummary>> GetByAuthorsAsync(
        FilterDTO filterDTO,
        PaginationDTO paginationDTO,
        AuthorFilterDTO authorFilterDto,
        int currentPublicationId,
        CancellationToken cancellationToken = default);
}
