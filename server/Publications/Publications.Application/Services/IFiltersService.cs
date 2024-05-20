using Publications.Domain.Publications;

namespace Publications.Application.Services;

public interface IFiltersService
{
    Task<IReadOnlyCollection<FilterGroup>> GetFiltersForPublicationsAsync(
        IEnumerable<Publication> publications);
    
    IReadOnlyCollection<Publication> AssignFiltersToPublicationsAsync(
        ICollection<Publication> publications, ICollection<FilterGroup> filters);
}