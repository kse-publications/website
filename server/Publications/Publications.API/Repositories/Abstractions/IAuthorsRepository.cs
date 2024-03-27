using Publications.API.Models;

namespace Publications.API.Repositories.Abstractions;

public interface IAuthorsRepository
{
    Task InsertOrUpdateAsync(
        IEnumerable<Author> authors, 
        CancellationToken cancellationToken = default);
}