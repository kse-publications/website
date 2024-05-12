using Publications.Domain.Filters;
using Publications.Domain.Publications;

namespace Publications.Application.Repositories;

public interface IPublicationsCommandRepository: IEntityRepository<Publication>
{
    Task ReplaceFiltersAsync(
        IEnumerable<FilterGroup> filters, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns all the filters available.
    /// </summary>
    Task<IReadOnlyCollection<FilterGroup>> GetFiltersAsync(
        CancellationToken cancellationToken = default);
}