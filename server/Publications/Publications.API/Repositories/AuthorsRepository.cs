using Publications.API.Models;
using Publications.API.Repositories.Abstractions;
using Redis.OM;
using Redis.OM.Searching;

namespace Publications.API.Repositories;

public class AuthorsRepository: IAuthorsRepository
{
    private readonly IRedisCollection<Author> _authors;

    public AuthorsRepository(RedisConnectionProvider provider)
    {
        _authors = provider.RedisCollection<Author>();
    }

    public async Task InsertOrUpdateAsync(
        IEnumerable<Author> authors, CancellationToken cancellationToken = default)
    {
        await _authors.InsertAsync(authors);
    }
}