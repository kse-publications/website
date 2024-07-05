using System.ComponentModel.DataAnnotations;

namespace Publications.Application.DTOs.Request;

/// <summary>
/// Basic pagination DTO.
/// </summary>
public record PaginationDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
    public int Page { get; init; } = 1;

    [Range(1, 101, ErrorMessage = "PageSize must be between 1 and 100 (inclusive).")]
    public int PageSize { get; init; } = 20;
    
    public int GetOffset() => PageSize * (Page - 1);
}