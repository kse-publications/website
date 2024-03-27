using Publications.API.Models;

namespace Publications.API.Repositories.Abstractions;

public interface IPublicationsSourceRepository
{
    Task<IReadOnlyCollection<Publication>> GetPublicationsAsync();
    Task<IReadOnlyCollection<Author>> GetAuthorsAsync();
    Task<IReadOnlyCollection<Publisher>> GetPublishersAsync();
}