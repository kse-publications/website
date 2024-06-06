using Publications.Application.DTOs;
using Publications.Domain.Filters;
using Publications.Domain.Publications;
using Redis.OM.Searching;

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
    
    Task<PaginatedCollection<PublicationSummary>> GetRelatedByAuthorsAsync(
        int currentPublicationId,
        PaginationDTO paginationDto, 
        AuthorFilterDTO authorFilterDto, 
        CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<PublicationSummary>> GetSimilarAsync(
        int currentPublicationId,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<FilterGroup>> GetFiltersAsync(
        FilterDTO filterDTO,
        PaginationDTO paginationDTO,
        SearchDTO searchDTO,
        CancellationToken cancellationToken = default);
}