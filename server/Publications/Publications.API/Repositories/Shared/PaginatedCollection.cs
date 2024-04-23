
namespace Publications.API.Repositories.Shared;

public record PaginatedCollection<T>(
    IReadOnlyCollection<T> Items, int ResultCount, int TotalCount);