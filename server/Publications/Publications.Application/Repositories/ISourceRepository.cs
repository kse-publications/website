using Publications.Domain.Authors;
using Publications.Domain.Publications;
using Publications.Domain.Publishers;

namespace Publications.Application.Repositories;

public interface ISourceRepository
{
    Task<IReadOnlyCollection<Publication>> GetPublicationsAsync();
    Task<IReadOnlyCollection<Collection>> GetCollectionsAsync();
    Task<IReadOnlyCollection<Author>> GetAuthorsAsync();
    Task<IReadOnlyCollection<Publisher>> GetPublishersAsync();
}