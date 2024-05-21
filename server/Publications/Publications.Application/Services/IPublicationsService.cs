using Publications.Application.DTOs;
using Publications.Domain.Publications;

namespace Publications.Application.Services;

public interface IPublicationsService
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
    
    Task<Publication?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<FilterGroup>> GetFiltersAsync(
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<FilterGroup>> GetFiltersV2Async(
        FilterDTOV2 filterDtov2,
        PaginationDTO paginationDTO,
        SearchDTO searchDTO,
        CancellationToken cancellationToken = default);

    Task<PaginatedCollection<PublicationSummary>> GetRelatedByAuthorsAsync(
        int currentPublicationId,
        PaginationDTO paginationDto, 
        AuthorFilterDTO authorFilterDto, 
        CancellationToken cancellationToken);
}