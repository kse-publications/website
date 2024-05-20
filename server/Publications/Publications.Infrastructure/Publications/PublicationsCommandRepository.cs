using Publications.Application.Repositories;
using Publications.Domain.Publications;
using Publications.Infrastructure.Shared;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace Publications.Infrastructure.Publications;

public class PublicationsCommandRepository : 
    EntityRepository<Publication>, IPublicationsCommandRepository
{
    private readonly IRedisCollection<Publication> _publications;
    private readonly IRedisCollection<FilterGroup> _filters;
    
    public PublicationsCommandRepository(
        IRedisConnectionProvider connectionProvider) : base(connectionProvider)
    {
        _publications = connectionProvider.RedisCollection<Publication>();
        _filters = connectionProvider.RedisCollection<FilterGroup>();
    }

    public async Task ReplaceFiltersAsync(
        IEnumerable<FilterGroup> filters,
        CancellationToken cancellationToken = default)
    {
        await _filters.DeleteAsync(await _filters.ToListAsync());
        await _filters.InsertAsync(filters);
    }

    public async Task<IReadOnlyCollection<FilterGroup>> GetFiltersAsync(
        CancellationToken cancellationToken = default)
    {
        return (await _filters.ToListAsync()).AsReadOnly();
    }
}