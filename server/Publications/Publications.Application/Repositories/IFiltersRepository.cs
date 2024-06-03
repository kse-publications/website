using Publications.Application.DTOs;
using Publications.Domain.Filters;

namespace Publications.Application.Repositories;

public interface IFiltersRepository
{
    Task<IReadOnlyCollection<FilterGroup>> GetAllAsync(
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns filters with the count of matched publications.
    /// </summary>
    /// <returns> Dictionary where key - filter value, value - count of matched publications. </returns>
    Task<Dictionary<string, int>> GetFiltersWithMatchedCountAsync(
        FilterDTO filterDTO, 
        SearchDTO searchDTO,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Replaces all existing filters with the provided ones.
    /// </summary>
    Task ReplaceWithNewAsync(
        IEnumerable<FilterGroup> filters,
        CancellationToken cancellationToken = default);
}