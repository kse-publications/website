using Publications.Application.DTOs.Request;
using Publications.Application.DTOs.Response;
using Publications.Domain.Publications;
using SearchDTO = Publications.Application.DTOs.Request.SearchDTO;

namespace Publications.Application.Repositories;

/// <summary>
/// Contract for a repository that manages <see cref="Publication"/>s.
/// </summary>
public interface IPublicationsRepository : IEntityRepository<Publication>
{
    Task<PaginatedCollection<PublicationSummary>> GetAllAsync(
        FilterDTO filterDTO,
        PaginationDTO paginationDTO,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns publications that match the provided search and filter criteria.
    /// </summary>
    Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        FilterDTO filterDTO,
        PaginationDTO paginationDTO,
        SearchDTO searchDTO,
        CancellationToken cancellationToken = default);
    
    // Task<Publication[]> GetAllMatchedByKeywordsAsync(
    //     string[] keywords, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns paginated collection of publications
    /// that have one or more authors in common with the provided publication.
    /// </summary>
    Task<PaginatedCollection<PublicationSummary>> GetRelatedByAuthorsAsync(
        int currentPublicationId,
        PaginationDTO paginationDTO,
        AuthorFilterDTO authorFilterDto,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<PublicationSummary>> GetSimilarAsync(
        int currentPublicationId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns paginated collection of publications
    /// that are part of the provided collection.
    /// </summary>
    Task<PaginatedCollection<PublicationSummary>> GetFromCollectionAsync(
        int collectionId,
        PaginationDTO paginationDTO,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<SyncEntityMetadata>> GetAllSyncMetadataAsync();
    
    Task<Publication[]> GetNonVectorizedAsync();
    // Task<Publication[]> GetWithNonPopulatedCollectionsAsync();
    
    Task UpdateAsync(
        int publicationId,
        string propertyName,
        string newValue,
        CancellationToken cancellationToken = default);
    
     Task<PublicationSummary[]> GetTopPublicationsByRecentViews(int count = 4);
}
