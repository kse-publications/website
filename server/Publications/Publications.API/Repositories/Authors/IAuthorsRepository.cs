using Publications.API.Models;

namespace Publications.API.Repositories.Authors;

public interface IAuthorsRepository
{
    Task InsertOrUpdateAsync(
        IEnumerable<Author> authors, 
        CancellationToken cancellationToken = default);
}