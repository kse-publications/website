using Publications.API.DTOs;
using Publications.API.Models;
using Publications.API.Repositories;
using Publications.API.Repositories.Publications;
using Publications.API.Repositories.Shared;

namespace Publications.API.Services;

public class PublicationsService: IPublicationsService
{
    private readonly IPublicationsRepository _publicationsRepository;

    public PublicationsService(IPublicationsRepository publicationsRepository)
    {
        _publicationsRepository = publicationsRepository;
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetAllAsync(
        PublicationsPaginationFilterDTO paginationDTO, CancellationToken cancellationToken = default)
    {
        PaginatedCollection<Publication> publications = await _publicationsRepository
            .GetAllAsync(paginationDTO, cancellationToken);
        
        return GetPublicationsSummaries(publications);
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        PublicationsPaginationSearchDTO paginationSearchDTO, 
        CancellationToken cancellationToken = default)
    {
        const int minSearchTermLength = 2;
        
        if (paginationSearchDTO.SearchTerm.Length < minSearchTermLength)
            return EmptyResponse;
        
        PaginatedCollection<Publication> matchedPublications = await _publicationsRepository
            .GetBySearchAsync(paginationSearchDTO, cancellationToken);
        
        return GetPublicationsSummaries(matchedPublications);
    }

    public async Task<Publication?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        return await _publicationsRepository.GetByIdAsync(id, cancellationToken);
    }

    private static PaginatedCollection<PublicationSummary> GetPublicationsSummaries(
        PaginatedCollection<Publication> publications)
    {
        IReadOnlyCollection<PublicationSummary> summaries = publications.Items
            .Select(PublicationSummary.FromPublication)
            .ToList()
            .AsReadOnly();
        
        return new PaginatedCollection<PublicationSummary>(
            Items: summaries,
            ResultCount: publications.ResultCount,
            TotalCount: publications.TotalCount);
    }
    
    private static PaginatedCollection<PublicationSummary> EmptyResponse => 
        new PaginatedCollection<PublicationSummary>(
            Items: new List<PublicationSummary>(), ResultCount: 0, TotalCount: 0);
}