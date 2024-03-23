namespace Publications.API.Abstractions;

public interface IPublicationsRepository
{
    Task<IReadOnlyCollection<IPublication>> GetAllAsync(
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<IPublication>> GetByFullTextSearchAsync(
        string searchTerm, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<IPublication>> GetByAutoCompleteAsync(
        string searchTerm, CancellationToken cancellationToken = default);
    
    Task<IPublicationsDetails> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default);
    
    Task InsertOrUpdateAsync(
        IEnumerable<IPublicationsDetails> publications,
        CancellationToken cancellationToken = default);
}