using System.ComponentModel.DataAnnotations;

namespace Publications.API.DTOs;

/// <summary>
/// Basic pagination DTO.
/// </summary>
public record PaginationFilterDTO: FilterDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
    public int Page { get; init; } = 1;

    [Range(1, 101, ErrorMessage = "PageSize must be between 1 and 100 (inclusive).")]
    public int PageSize { get; init; } = 20;
    
    public bool Ascending { get; init; } = false;
}