
namespace Publications.API.Repositories;

public record PaginatedCollection<T>(
    IReadOnlyCollection<T> Items, int ResultCount, int TotalCount);