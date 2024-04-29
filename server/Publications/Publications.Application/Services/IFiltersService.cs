using Publications.Domain.Filters;
using Publications.Domain.Publications;

namespace Publications.Application.Services;

public interface IFiltersService
{
    Task<IReadOnlyCollection<FilterGroup>> GetFiltersForPublicationsAsync(
        IEnumerable<Publication> publications);
    
    Task<ICollection<Publication>> AssignFiltersToPublicationsAsync(
        ICollection<Publication> publications, IReadOnlyCollection<FilterGroup> filters);
}