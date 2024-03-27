using Publications.API.Models;

namespace Publications.API.Repositories.Publishers;

public interface IPublishersRepository
{
    Task InsertOrUpdateAsync(
        IEnumerable<Publisher> publishers, 
        CancellationToken cancellationToken = default);
}