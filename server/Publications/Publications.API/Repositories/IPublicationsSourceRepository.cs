using Publications.API.Models;

namespace Publications.API.Repositories;

public interface IPublicationsSourceRepository
{
    Task<IReadOnlyCollection<Publication>> GetPublicationsAsync();
}