using Publications.Domain.Filters;

namespace Publications.Application.Repositories;

public interface IFiltersRepository
{
    Task<IReadOnlyCollection<FilterGroup>> GetFiltersAsync(
        CancellationToken cancellationToken = default);
    
    Task ReplaceWithAsync(
        IEnumerable<FilterGroup> filters, 
        CancellationToken cancellationToken = default);
}