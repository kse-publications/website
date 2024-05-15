using Publications.Application;
using Publications.Application.DTOs;
using Publications.Application.Repositories;
using Publications.Application.Services;
using Publications.Domain.Filters;
using Publications.Domain.Publications;
using Publications.Infrastructure.Shared;

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
        FilterDTO filterDTO, PaginationDTO paginationDTO,
        CancellationToken cancellationToken = default)
    {
        PaginatedCollection<PublicationSummary> publications = await 
            _publicationsQueryRepository.GetAllAsync(
                filterDTO, paginationDTO, cancellationToken);
        
        return publications;
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetByAuthorsAsync(
        FilterDTO filterDto, PaginationDTO paginationDto, AuthorFilterDTO authorFilterDto, int currentPublicationId,
        CancellationToken cancellationToken = default)
    {
        PaginatedCollection<PublicationSummary> recomandsbyauthors = await
            _publicationsQueryRepository.GetByAuthorsAsync(
                filterDto, paginationDto, authorFilterDto, currentPublicationId, cancellationToken);
        return recomandsbyauthors;
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        FilterDTO filterDTO, PaginationDTO paginationDTO, SearchDTO searchDTO, 
        CancellationToken cancellationToken = default)
    {
        PaginatedCollection<PublicationSummary> matchedPublications = await 
            _publicationsQueryRepository.GetBySearchAsync(
                filterDTO, paginationDTO, searchDTO, cancellationToken);

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
        FilterDTOV2 filterDtoV2, PaginationDTO paginationDTO, SearchDTO searchDTO,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<FilterGroup> filterGroups = await
            _publicationsCommandRepository.GetFiltersAsync(cancellationToken);
        
        Dictionary<string, int> filtersValuesWithMatchesCount = await
            _publicationsQueryRepository.GetFiltersCountAsync(
                filterDtoV2, paginationDTO, searchDTO, cancellationToken);
        
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