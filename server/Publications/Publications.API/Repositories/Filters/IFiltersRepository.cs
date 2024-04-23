using Publications.API.Models;

namespace Publications.API.Repositories.Filters;

public interface IFiltersRepository
{
    Task<IReadOnlyCollection<FilterGroup>> GetFiltersAsync(
        CancellationToken cancellationToken = default);
    
    Task InsertOrUpdateAsync(
        IEnumerable<FilterGroup> filters, 
        CancellationToken cancellationToken = default);
}