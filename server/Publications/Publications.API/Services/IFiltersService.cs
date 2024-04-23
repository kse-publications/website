using Publications.API.Models;

namespace Publications.API.Services;

public interface IFiltersService
{
    Task<IReadOnlyCollection<FilterGroup>> GetFiltersForPublicationsAsync(
        IEnumerable<Publication> publications);
    
    Task<ICollection<Publication>> AssignFiltersToPublicationsAsync(
        ICollection<Publication> publications, IReadOnlyCollection<FilterGroup> filters);
}