using Publications.Application.DTOs.Request;
using Publications.Domain.Filters;
using SearchDTO = Publications.Application.DTOs.Request.SearchDTO;

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