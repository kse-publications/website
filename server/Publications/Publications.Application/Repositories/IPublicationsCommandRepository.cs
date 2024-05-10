using Publications.Domain.Filters;
using Publications.Domain.Publications;

namespace Publications.Application.Repositories;

public interface IPublicationsCommandRepository: IEntityRepository<Publication>
{
    Task ReplaceFiltersAsync(
        IEnumerable<FilterGroup> filters, 
        CancellationToken cancellationToken = default);
}