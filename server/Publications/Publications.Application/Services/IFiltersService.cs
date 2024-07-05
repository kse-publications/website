using Publications.Domain.Filters;
using Publications.Domain.Publications;

namespace Publications.Application.Services;

public interface IFiltersService
{
    Task<FilterGroup[]> GetFiltersForPublicationsAsync(
        IEnumerable<Publication> publications);
    
    IList<Publication> AssignFiltersToPublications(
        IList<Publication> publications, IList<FilterGroup> filters);
}