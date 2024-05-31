using Publications.Domain.Collections;
using Publications.Domain.Publications;

namespace Publications.Application.Repositories;

public interface ISourceRepository
{
    Task<IReadOnlyCollection<Publication>> GetPublicationsAsync();
    Task<IReadOnlyCollection<Collection>> GetCollectionsAsync();
    Task<IReadOnlyCollection<Author>> GetAuthorsAsync();
    Task<IReadOnlyCollection<Publisher>> GetPublishersAsync();
}