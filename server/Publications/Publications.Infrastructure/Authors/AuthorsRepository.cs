using Publications.Application.Repositories;
using Publications.Domain.Authors;
using Publications.Infrastructure.Shared;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace Publications.Infrastructure.Authors;

public class AuthorsRepository: EntityRepository<Author>, IAuthorsRepository
{
    private readonly IRedisCollection<Author> _authors;

    public AuthorsRepository(IRedisConnectionProvider provider)
        : base(provider)
    {
        _authors = provider.RedisCollection<Author>();
    }
}