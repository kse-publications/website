using Publications.API.Models;
using Publications.API.Repositories.Shared;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace Publications.API.Repositories.Authors;

public class AuthorsRepository: EntityRepository<Author>, IAuthorsRepository
{
    private readonly IRedisCollection<Author> _authors;

    public AuthorsRepository(IRedisConnectionProvider provider)
        : base(provider)
    {
        _authors = provider.RedisCollection<Author>();
    }
}