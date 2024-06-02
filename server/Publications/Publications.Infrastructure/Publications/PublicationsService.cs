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

    public Task<PaginatedCollection<PublicationSummary>> GetAllAsync(
        FilterDTO filterDTO, PaginationDTO paginationDTO,
        CancellationToken cancellationToken = default)
    {
       return _publicationsQueryRepository.GetAllAsync(
           filterDTO, paginationDTO, cancellationToken);
    }

    public Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        FilterDTO filterDTO, PaginationDTO paginationDTO, SearchDTO searchDTO, 
        CancellationToken cancellationToken = default)
    {
        return _publicationsQueryRepository.GetBySearchAsync(
            filterDTO, paginationDTO, searchDTO, cancellationToken);
    }

    public Task<Publication?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        return _publicationsCommandRepository.GetByIdAsync(id, cancellationToken);
    }
    
    public Task<PaginatedCollection<PublicationSummary>> GetRelatedByAuthorsAsync(
        int currentPublicationId, PaginationDTO paginationDto, AuthorFilterDTO authorFilterDto,
        CancellationToken cancellationToken = default)
    {
        return _publicationsQueryRepository.GetRelatedByAuthorsAsync(
            currentPublicationId, paginationDto, authorFilterDto, cancellationToken);
    }

    public async Task<IReadOnlyCollection<FilterGroup>> GetFiltersAsync(
        FilterDTO filterDTO, PaginationDTO paginationDTO, SearchDTO searchDTO,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<FilterGroup> filterGroups = await
            _publicationsCommandRepository.GetFiltersAsync(cancellationToken);
        
        Dictionary<string, int> filtersValuesWithMatchesCount = await
            _publicationsQueryRepository.GetFiltersWithMatchedCountAsync(
                filterDTO, searchDTO, cancellationToken);
        
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
}