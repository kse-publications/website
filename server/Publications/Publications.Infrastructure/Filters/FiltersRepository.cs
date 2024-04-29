using Publications.Application.Repositories;
using Publications.Domain.Filters;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace Publications.Infrastructure.Filters;

public class FiltersRepository: IFiltersRepository
{
    private readonly IRedisConnectionProvider _redisConnectionProvider;
    private readonly IRedisCollection<FilterGroup> _filters;

    public FiltersRepository(
        IRedisConnectionProvider redisConnectionProvider)
    {
        _redisConnectionProvider = redisConnectionProvider;
        _filters = redisConnectionProvider.RedisCollection<FilterGroup>();
    }

    public async Task<IReadOnlyCollection<FilterGroup>> GetFiltersAsync(
        CancellationToken cancellationToken)
    {
        return (await _filters.ToListAsync()).AsReadOnly();
    }
    
    public async Task InsertOrUpdateAsync(
        IEnumerable<FilterGroup> filters, 
        CancellationToken cancellationToken)
    {
        await _filters.DeleteAsync(await _filters.ToListAsync());
        await _filters.InsertAsync(filters);
    }
}