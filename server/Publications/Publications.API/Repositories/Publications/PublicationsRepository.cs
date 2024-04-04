using Publications.API.DTOs;
using Publications.API.Models;
using Redis.OM;
using Redis.OM.Searching;
using Redis.OM.Searching.Query;

namespace Publications.API.Repositories.Publications;

public class PublicationsRepository: IPublicationsRepository
{
    private readonly RedisConnectionProvider _redisConnectionProvider;
    private readonly IRedisCollection<Publication> _publications;

    public PublicationsRepository(RedisConnectionProvider redisConnectionProvider)
    {
        _redisConnectionProvider = redisConnectionProvider;
        _publications = redisConnectionProvider.RedisCollection<Publication>();
    }

    public async Task<PaginatedCollection<Publication>> GetAllAsync(
        PaginationFilterDTO paginationFilterDTO, CancellationToken cancellationToken = default)
    {
        IRedisCollection<Publication> sortedFilteredPublications = _publications
            .ApplyFiltering(paginationFilterDTO)
            .ApplySorting(paginationFilterDTO.SortBy, paginationFilterDTO.Ascending);
        
        return await sortedFilteredPublications.ApplyPagination(
            paginationFilterDTO.Page,
            paginationFilterDTO.PageSize, 
            totalMatches: await sortedFilteredPublications.CountAsync());
    }

    public async Task<PaginatedCollection<Publication>> GetBySearchAsync(
        PaginationSearchDTO paginationSearchDTO,
        CancellationToken cancellationToken = default)
    {
        string searchQuery =
            $"((((((@{nameof(Publication.Title)}:{paginationSearchDTO.SearchTerm}*) " +
            $"| (@{nameof(Publication.Title)}:{paginationSearchDTO.SearchTerm})) " +
            $"| (@{nameof(Publication.Abstract)}:{paginationSearchDTO.SearchTerm}*)) " +
            $"| (@{nameof(Publication.Abstract)}:{paginationSearchDTO.SearchTerm})) " +
            $"| (@{nameof(Publication.Publisher)}_{nameof(Publisher.Name)}:{paginationSearchDTO.SearchTerm})) " +
            $"| (@{nameof(Publication.Authors)}_{nameof(Author.Name)}:{paginationSearchDTO.SearchTerm}))";
        
        if (paginationSearchDTO.Filter != string.Empty)
        {
            searchQuery += $" (@{nameof(Publication.Type)}:{{{paginationSearchDTO.Filter}}})";
        }
        
        RedisQuery query = new("publication-idx") 
        { 
            QueryText = searchQuery
        };
        
        Task<SearchResponse<Publication>> matchedCountTask = _redisConnectionProvider.Connection
            .SearchAsync<Publication>(query);
        
        query.Limit = new SearchLimit
        {
            Number = paginationSearchDTO.PageSize,
            Offset = (paginationSearchDTO.Page - 1) * paginationSearchDTO.PageSize
        };
        
        Task<SearchResponse<Publication>> paginatedPublicationsTask = _redisConnectionProvider.Connection
            .SearchAsync<Publication>(query);
        
        await Task.WhenAll(matchedCountTask, paginatedPublicationsTask);
        IReadOnlyCollection<Publication> publications = (await paginatedPublicationsTask)
            .Documents.Values
            .ToList()
            .AsReadOnly();
        
        return new PaginatedCollection<Publication>(
            Items: publications,
            ResultCount: publications.Count,
            TotalCount: (int)(await matchedCountTask).DocumentCount);
    }

    public async Task<Publication?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        return await _publications.FindByIdAsync(id.ToString());
    }

    public async Task InsertOrUpdateAsync(
        IEnumerable<Publication> publications,
        CancellationToken cancellationToken = default)
    {
        await _publications.InsertAsync(publications);
    }
}