using Publications.API.Models;

namespace Publications.API.Repositories.Source;

public interface ISourceRepository
{
    Task<IReadOnlyCollection<Publication>> GetPublicationsAsync();
    Task<IReadOnlyCollection<Author>> GetAuthorsAsync();
    Task<IReadOnlyCollection<Publisher>> GetPublishersAsync();
}