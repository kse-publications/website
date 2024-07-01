using Publications.Application.DTOs.Request;
using Publications.Application.DTOs.Response;
using Publications.Application.Repositories;
using Publications.Application.Services;
using Publications.Domain.Filters;
using Publications.Domain.Publications;

namespace Publications.Infrastructure.Publications;

public class PublicationsService: IPublicationsService
{
    private readonly IPublicationsRepository _publicationsRepository;
    private readonly IFiltersRepository _filtersRepository;

    public PublicationsService(
        IPublicationsRepository publicationsRepository,
        IFiltersRepository filtersRepository)
    {
        _publicationsRepository = publicationsRepository;
        _filtersRepository = filtersRepository;
    }

    public Task<PaginatedCollection<PublicationSummary>> GetAllAsync(
        FilterDTO filterDTO, PaginationDTO paginationDTO,
        CancellationToken cancellationToken = default)
    {
       return _publicationsRepository.GetAllAsync(
           filterDTO, paginationDTO, cancellationToken);
    }

    public Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        FilterDTO filterDTO, PaginationDTO paginationDTO, SearchDTO searchDTO, 
        CancellationToken cancellationToken = default)
    {
        return _publicationsRepository.GetBySearchAsync(
            filterDTO, paginationDTO, searchDTO, cancellationToken);
    }

    public Task<Publication?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        return _publicationsRepository.GetByIdAsync(id, cancellationToken);
    }
    
    public Task<PaginatedCollection<PublicationSummary>> GetRelatedByAuthorsAsync(
        int currentPublicationId, PaginationDTO paginationDto, AuthorFilterDTO authorFilterDto,
        CancellationToken cancellationToken = default)
    {
        return _publicationsRepository.GetRelatedByAuthorsAsync(
            currentPublicationId, paginationDto, authorFilterDto, cancellationToken);
    }
    
    public Task<IReadOnlyCollection<PublicationSummary>> GetSimilarAsync(
        int currentPublicationId,
        CancellationToken cancellationToken = default)
    {
        return _publicationsRepository.GetSimilarAsync(
            currentPublicationId,  cancellationToken);
    } 

    public async Task<IReadOnlyCollection<FilterGroup>> GetFiltersAsync(
        FilterDTO filterDTO, PaginationDTO paginationDTO, SearchDTO searchDTO,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<FilterGroup> filterGroups = await
            _filtersRepository.GetAllAsync(cancellationToken);
        
        Dictionary<string, int> filtersValuesWithMatchesCount = await
            _filtersRepository.GetFiltersWithMatchedCountAsync(
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