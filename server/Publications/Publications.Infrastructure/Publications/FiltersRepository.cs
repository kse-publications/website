using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using Publications.Application.DTOs.Request;
using Publications.Application.Repositories;
using Publications.Domain.Filters;
using Publications.Domain.Publications;
using Publications.Domain.Shared;
using Publications.Infrastructure.Services.DbConfiguration;
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
        
        List<Task<Dictionary<string, int>>> aggregationTasks = Publication
            .GetEntityFilters()
            .Select(async entityFilter =>
            {
                Dictionary<int, int[]> filtersWithoutCurrentGroup = new(filterDTO.GetParsedFilters());
                filtersWithoutCurrentGroup.Remove(entityFilter.GroupId);
                var newFilter = FilterDTO.CreateFromFilters(filtersWithoutCurrentGroup);
                
                SearchQuery query = SearchQuery.CreateWithSearch(searchDTO.SearchTerm)
                    .Filter(newFilter.GetParsedFilters());

                const int maxAllowedLimit = 10_000;
                var searchResult = await ft.SearchAsync(RedisIndexVersionInfo.GetIndexName(typeof(Publication)), 
                    new Query(query.Build())
                        .LimitFields(Publication.GetSearchableFields())
                        .Limit(0, maxAllowedLimit)
                        .ReturnFields(entityFilter.PropertyName)
                        .Dialect(3));
                
                Dictionary<string, int> filtersCounts = searchResult.Documents
                    .GroupBy(d => d[entityFilter.PropertyName].ToString())
                    .ToDictionary(g => g.Key, g => g.Count());
                
                return filtersCounts;
            })
            .ToList();
        
        return (await Task.WhenAll(aggregationTasks))
            .SelectMany(dict => dict)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
}