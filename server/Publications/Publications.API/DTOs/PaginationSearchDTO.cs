
namespace Publications.API.DTOs;

/// <summary>
/// Basic pagination and search DTO.
/// </summary>
public record PaginationSearchDTO: PaginationDTO, ISearchDTO
{
    private readonly string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        init => _searchTerm = this.CleanSearchTerm(value);
    }
}

