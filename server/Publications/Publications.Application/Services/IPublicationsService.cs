using Publications.Application.DTOs;
using Publications.Domain.Filters;
using Publications.Domain.Publications;

namespace Publications.Application.Services;

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
    
    Task<IReadOnlyCollection<FilterGroup>> GetFiltersAsync(
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<FilterGroup>> GetFiltersV2Async(
        PaginationFilterSearchDtoV2 filterSearchDTO,
        CancellationToken cancellationToken = default);
}