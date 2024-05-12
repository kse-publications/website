using Publications.Application;
using Publications.Application.DTOs;
using Publications.Application.Repositories;
using Publications.Application.Services;
using Publications.Domain.Filters;
using Publications.Domain.Publications;

namespace Publications.Infrastructure.Publications;

public class PublicationsService: IPublicationsService
{
    private readonly IPublicationsQueryRepository _publicationsQueryRepository;
    private readonly IPublicationsCommandRepository _publicationsCommandRepository;

    public PublicationsService(
        IPublicationsQueryRepository publicationsQueryRepository,
        IPublicationsCommandRepository publicationsCommandRepository)
    {
        _publicationsQueryRepository = publicationsQueryRepository;
        _publicationsCommandRepository = publicationsCommandRepository;
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetAllAsync(
        PaginationFilterDTO paginationDTO, CancellationToken cancellationToken = default)
    {
        PaginatedCollection<PublicationSummary> publications = await 
            _publicationsQueryRepository.GetAllAsync(paginationDTO, cancellationToken);
        
        return publications;
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        PaginationFilterSearchDTO paginationSearchDTO, 
        CancellationToken cancellationToken = default)
    {
        PaginatedCollection<PublicationSummary> matchedPublications = await 
            _publicationsQueryRepository.GetBySearchAsync(paginationSearchDTO, cancellationToken);

        return matchedPublications;
    }

    public async Task<Publication?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        return await _publicationsCommandRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<FilterGroup>> GetFiltersAsync(
        CancellationToken cancellationToken = default)
    {
        return (await _publicationsCommandRepository.GetFiltersAsync(cancellationToken))
            .OrderBy(fg => fg.Id)
            .ToList()
            .AsReadOnly();
    }

    public async Task<IReadOnlyCollection<FilterGroup>> GetFiltersV2Async(
        PaginationFilterSearchDtoV2 filterSearchDTO, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<FilterGroup> filterGroups = await
            _publicationsCommandRepository.GetFiltersAsync(cancellationToken);
        
        Dictionary<string, int> filtersValuesWithMatchesCount = await
            _publicationsQueryRepository.GetFiltersCountAsync(filterSearchDTO, cancellationToken);
        
        return MatchFiltersCount(filterGroups, filtersValuesWithMatchesCount)
            .OrderBy(fg => fg.Id)
            .ToList()
            .AsReadOnly();
    }
    
    private static IReadOnlyCollection<FilterGroup> MatchFiltersCount(
        IReadOnlyCollection<FilterGroup> filterGroups,
        Dictionary<string, int> filtersValuesWithMatchesCount)
    {
        foreach (FilterGroup filterGroup in filterGroups)
        {
            foreach (Filter filter in filterGroup.Filters)
            {
                if (filtersValuesWithMatchesCount.
                    TryGetValue(filter.Value, out int matchesCount))
                {
                    filter.MatchedPublicationsCount = matchesCount;
                }
            }
        }

        return filterGroups;
    }

    private static PaginatedCollection<PublicationSummary> EmptyResponse => 
        new PaginatedCollection<PublicationSummary>(
            Items: new List<PublicationSummary>(), ResultCount: 0, TotalCount: 0);
}