namespace Publications.API.DTOs;

/// <summary>
/// Publications specific pagination and filter DTO.
/// </summary>
public record PublicationsPaginationFilterDTO: PaginationDTO
{
    public string Type { get; init; } = string.Empty;
    public int Year { get; init; }
    public string Language { get; init; } = string.Empty;
}

/// <summary>
/// Publications specific pagination, filter and search DTO.
/// </summary>
public record PublicationsPaginationSearchDTO 
    : PublicationsPaginationFilterDTO, ISearchDTO
{
    private readonly string _searchTerm = string.Empty;
    
    public string SearchTerm
    {
        get => _searchTerm;
        init => _searchTerm = this.CleanSearchTerm(value);
    }
}