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
    
    /// <summary>
    /// Returns the slug of every publication.
    /// </summary>
    Task<IReadOnlyCollection<string>> GetAllSlugsAsync(
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
    
    Task<PaginatedCollection<PublicationSummary>> GetRelatedByAuthorsAsync(
        int currentPublicationId,
        PaginationDTO paginationDTO,
        AuthorFilterDTO authorFilterDto,
        CancellationToken cancellationToken = default);
}
