using System.Text.Json;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Aggregation;
using Publications.Application;
using Publications.Application.DTOs;
using Publications.Application.Repositories;
using Publications.Domain.Authors;
using Publications.Domain.Filters;
using Publications.Domain.Publications;
using Publications.Domain.Publishers;
using Publications.Infrastructure.Shared;
using StackExchange.Redis;

namespace Publications.Infrastructure.Publications;

public class PublicationsQueryRepository: IPublicationsQueryRepository
{
    private readonly IDatabase _db;

    public PublicationsQueryRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        _db = connectionMultiplexer.GetDatabase();
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetAllAsync(
        FilterDTO filterDTO, PaginationDTO paginationDTO,
        CancellationToken cancellationToken = default)
    {
        SearchCommands ft = _db.FT();
        SearchQuery query = SearchQuery.CreateWithFilter(filterDTO);
        
        AggregationResult aggregationResult = await ft.AggregateAsync(Publication.IndexName,
            new AggregationRequest(query.Build())
                .Load(
                    new FieldName(nameof(Publication.Slug)),
                    new FieldName(nameof(Publication.Title)),
                    new FieldName(nameof(Publication.Type)),
                    new FieldName(nameof(Publication.Year)),
                    new FieldName(PublicationAuthorsName),
                    new FieldName(PublicationPublisherName))
                .SortBy(
                    new SortedField($"@{nameof(Publication.Year)}", SortedField.SortOrder.DESC),
                    new SortedField($"@{nameof(Publication.Id)}", SortedField.SortOrder.DESC))
                .Limit(
                    offset: paginationDTO.PageSize * (paginationDTO.Page - 1),
                    count: paginationDTO.PageSize)
                .Dialect(3));
        
        IReadOnlyCollection<PublicationSummary> publications = 
            MapToPublicationSummaries(aggregationResult.GetResults())
                .AsReadOnly();
        
        return new PaginatedCollection<PublicationSummary>(
            Items: publications,
            TotalCount: (int)aggregationResult.TotalResults,
            ResultCount: publications.Count);
    }


    public async Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        FilterDTO filterDTO, PaginationDTO paginationDTO, SearchDTO searchDTO,
        CancellationToken cancellationToken = default)
    {
        SearchCommands ft = _db.FT();
        
        SearchFieldName[] searchFields = Publication.GetSearchableFields()
            .Select(fieldName => new SearchFieldName(fieldName))
            .ToArray();

        SearchQuery query = SearchQuery
            .CreateWithSearch(
                searchDTO.SearchTerm, searchFields)
            .Filter(filterDTO);
        
        var searchResult = await ft.SearchAsync(Publication.IndexName, 
            new Query(query.Build())
                .Limit(
                    offset: paginationDTO.PageSize * (paginationDTO.Page - 1),
                    count: paginationDTO.PageSize)
                .Dialect(3));
        
        IReadOnlyCollection<PublicationSummary> publications = searchResult
            .ToJson()
            .Select(json => PublicationSummary
                .FromPublication(JsonSerializer
                    .Deserialize<Publication[]>(json)!.First()))
            .ToList()
            .AsReadOnly();
        
        return new PaginatedCollection<PublicationSummary>(
            Items: publications,
            TotalCount: (int)searchResult.TotalResults,
            ResultCount: publications.Count);
    }

    public async Task<Dictionary<string, int>> GetFiltersCountAsync(
        FilterDTOV2 filterDtoV2, PaginationDTO paginationDTO, SearchDTO searchDTO,
        CancellationToken cancellationToken = default)
    {
        SearchCommands ft = _db.FT();
        SearchFieldName[] searchFields = Publication.GetSearchableFields()
            .Select(fieldName => new SearchFieldName(fieldName))
            .ToArray();
        
        List<Task<Dictionary<string, int>>> aggregationTasks = Publication
            .GetEntityFilters()
            .Select(async entityFilter =>
            {
                Dictionary<int, int[]> filtersWithoutCurrentGroup = new(filterDtoV2
                    .GetParsedFilters());
                
                filtersWithoutCurrentGroup.Remove(entityFilter.GroupId);
                
                var newFilter = FilterDTOV2.CreateFromFilters(
                    filtersWithoutCurrentGroup);
                
                SearchQuery query = SearchQuery
                    .CreateWithSearch(searchDTO.SearchTerm, searchFields)
                    .Filter(newFilter);
                
                var result = await ft.AggregateAsync(Publication.IndexName,
                    new AggregationRequest(query.Build())
                        .Load(new FieldName(entityFilter.PropertyName))
                        .GroupBy($"@{entityFilter.PropertyName}", 
                            Reducers.Count().As("count"))
                        .Dialect(3));

                return result.GetResults()
                    .Select(dict => new
                    {
                        Value = dict[entityFilter.PropertyName].ToString(),
                        Count = (int)dict["count"]
                    })
                    .ToDictionary(x => x.Value, x => x.Count);  
            })
            .ToList();


        return (await Task.WhenAll(aggregationTasks))
            .SelectMany(dict => dict)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private static List<PublicationSummary> MapToPublicationSummaries(
        IEnumerable<Dictionary<string, RedisValue>> aggregationResults)
    {
        return aggregationResults.Select(result => new PublicationSummary
        {
            Slug = result[nameof(Publication.Slug)]!,
            Title = JsonSerializer.Deserialize<string[]>(result[nameof(Publication.Title)]!)!.First(),
            Type = result[nameof(Publication.Type)]!,
            Year = (int)result[nameof(Publication.Year)]!,
            Authors = JsonSerializer.Deserialize<string[]>(result[PublicationAuthorsName]!)!,
            Publisher = JsonSerializer.Deserialize<string[]>(result[PublicationPublisherName]!)!.First()
        }).ToList();
    }

    private static string PublicationAuthorsName => 
        $"{nameof(Publication.Authors)}_{nameof(Author.Name)}";
    
    private static string PublicationPublisherName =>
        $"{nameof(Publication.Publisher)}_{nameof(Publisher.Name)}";
    
    private static string PublicationFiltersId =>
        $"{nameof(Publication.Filters)}_{nameof(Filter.Id)}";
}