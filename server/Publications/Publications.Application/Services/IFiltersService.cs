using Publications.Domain.Filters;
using Publications.Domain.Publications;

namespace Publications.Application.Services;

public interface IFiltersService
{
    Task<IReadOnlyCollection<FilterGroup>> GetFiltersForPublicationsAsync(
        IEnumerable<Publication> publications);
    
    IList<Publication> AssignFiltersToPublications(
        ICollection<Publication> publications, ICollection<FilterGroup> filters);
}