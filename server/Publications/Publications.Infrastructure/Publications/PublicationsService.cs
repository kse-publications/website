using Publications.Application;
using Publications.Application.DTOs;
using Publications.Application.Repositories;
using Publications.Application.Services;
using Publications.Domain.Publications;

namespace Publications.Infrastructure.Publications;

public class PublicationsService: IPublicationsService
{
    private readonly IPublicationsRepository _publicationsRepository;

    public PublicationsService(IPublicationsRepository publicationsRepository)
    {
        _publicationsRepository = publicationsRepository;
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetAllAsync(
        PaginationFilterDTO paginationDTO, CancellationToken cancellationToken = default)
    {
        PaginatedCollection<PublicationSummary> publications = await _publicationsRepository
            .GetAllAsync(paginationDTO, cancellationToken);
        
        return publications;
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        PaginationFilterSearchDTO paginationSearchDTO, 
        CancellationToken cancellationToken = default)
    {
        const int minSearchTermLength = 2;
        
        if (paginationSearchDTO.SearchTerm.Length < minSearchTermLength)
            return EmptyResponse;
        
        PaginatedCollection<PublicationSummary> matchedPublications = await _publicationsRepository
            .GetBySearchAsync(paginationSearchDTO, cancellationToken);

        return matchedPublications;
    }

    public async Task<Publication?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        return await _publicationsRepository.GetByIdAsync(id, cancellationToken);
    }
    
    private static PaginatedCollection<PublicationSummary> EmptyResponse => 
        new PaginatedCollection<PublicationSummary>(
            Items: new List<PublicationSummary>(), ResultCount: 0, TotalCount: 0);
}