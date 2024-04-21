namespace Publications.API.DTOs;

/// <summary>
/// Exposes a search term property. Has <see cref="SearchDTOExtensions"/> methods.
/// </summary>
public interface ISearchDTO
{
    string SearchTerm { get; init; }
}