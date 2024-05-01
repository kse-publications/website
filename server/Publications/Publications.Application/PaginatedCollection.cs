
namespace Publications.Application;

public record PaginatedCollection<T>(
    IReadOnlyCollection<T> Items, int ResultCount, int TotalCount);