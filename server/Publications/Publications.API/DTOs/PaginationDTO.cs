using Publications.API.Models;

namespace Publications.API.DTOs;

public record PaginationDTO(
    int Page = 1,
    int PageSize = 20, 
    string SortBy = nameof(Publication.LastModified),
    bool Ascending = false);