using Publications.API.Models;

namespace Publications.API.Repositories;

public interface IPublicationsRepository
{
    Task<IReadOnlyCollection<Publication>> GetAllAsync(
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<Publication>> GetByFullTextSearchAsync(
        string searchTerm, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<Publication>> GetByAutoCompleteAsync(
        string searchTerm, CancellationToken cancellationToken = default);
    
    Task<Publication> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default);
    
    Task InsertOrUpdateAsync(
        IEnumerable<Publication> publications,
        CancellationToken cancellationToken = default);
}