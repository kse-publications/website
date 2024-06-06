
namespace Publications.Application;

public record PaginatedCollection<T>(
    IReadOnlyCollection<T> Items,
    int ResultCount,
    int TotalCount)
{
    public static PaginatedCollection<T> GetEmpty() 
        => new(Array.Empty<T>(), 0, 0);
}