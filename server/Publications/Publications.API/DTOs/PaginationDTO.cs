using System.ComponentModel.DataAnnotations;
using Publications.API.Models;

namespace Publications.API.DTOs;

public record PaginationDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
    public int Page { get; init; } = 1;

    [Range(1, 101, ErrorMessage = "PageSize must be between 1 and 100 (inclusive).")]
    public int PageSize { get; init; } = 20;

    public string SortBy { get; init; } = nameof(Publication.LastModified);
    public bool Ascending { get; init; } = false;
}