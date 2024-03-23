using System.Collections;
using Publications.API.Models;

namespace Publications.API.Repositories;

public record PaginatedCollection<T>(
    IReadOnlyCollection<T> Items, int Count) : IEnumerable<T>
{
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}