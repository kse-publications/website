using Publications.Domain.Collections;
using Publications.Domain.Publications;

namespace Publications.Application.Repositories;

public interface ISourceRepository
{
    /// <summary>
    /// Returns all publications. Hydrates authors and publishers. No collections.
    /// </summary>
    Task<IReadOnlyCollection<Publication>> GetPublicationsAsync();
    
    /// <summary>
    /// Returns all collections. Does not hydrate PublicationIds.
    /// </summary>
    Task<IReadOnlyCollection<Collection>> GetCollectionsAsync();
    
    /// <summary>
    /// Returns joined publications and collections.
    /// </summary>
    Task<(IReadOnlyCollection<Publication>, IReadOnlyCollection<Collection>)> GetPublicationsAndCollectionsAsync();
    
    /// <summary>
    /// Returns all authors.
    /// </summary>
    Task<IReadOnlyCollection<Author>> GetAuthorsAsync();
    
    /// <summary>
    /// Returns all publishers.
    /// </summary>
    Task<IReadOnlyCollection<Publisher>> GetPublishersAsync();
}