using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Aggregation;
using Publications.Application.DTOs;
using Publications.Application.DTOs.Request;
using Publications.Application.Repositories;
using Publications.Domain.Filters;
using Publications.Domain.Publications;
using Publications.Domain.Shared;
using Publications.Infrastructure.Shared;
using Redis.OM;
using Redis.OM.Searching;
using StackExchange.Redis;

namespace Publications.Infrastructure.Publications;

public class FiltersRepository: IFiltersRepository
{
    private readonly IRedisCollection<FilterGroup> _filters;
    private readonly IDatabase _db;
    
    public FiltersRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        var connectionProvider = new RedisConnectionProvider(connectionMultiplexer); 
        _filters = connectionProvider.RedisCollection<FilterGroup>();
        _db = connectionMultiplexer.GetDatabase();
    }
    
    public async Task<IReadOnlyCollection<FilterGroup>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return (await _filters.ToListAsync()).AsReadOnly();
    }

    public async Task ReplaceWithNewAsync(
        IEnumerable<FilterGroup> filters,
        CancellationToken cancellationToken = default)
    {
        await _filters.DeleteAsync(await _filters.ToListAsync());
        await _filters.InsertAsync(filters);
    }
    
    public async Task<Dictionary<string, int>> GetFiltersWithMatchedCountAsync(
        FilterDTO filterDTO, SearchDTO searchDTO,
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
                Dictionary<int, int[]> filtersWithoutCurrentGroup = new(filterDTO
                    .GetParsedFilters());
                
                filtersWithoutCurrentGroup.Remove(entityFilter.GroupId);
                
                var newFilter = FilterDTO.CreateFromFilters(
                    filtersWithoutCurrentGroup);
                
                SearchQuery query = SearchQuery
                    .CreateWithSearch(searchDTO.SearchTerm, searchFields)
                    .Filter(newFilter);
                
                var result = await ft.AggregateAsync(Entity.IndexName<Publication>(),
                    new AggregationRequest(query.Build())
                        .Load(new FieldName(entityFilter.PropertyName))
                        .GroupBy($"@{entityFilter.PropertyName}", Reducers.Count().As("count"))
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
}