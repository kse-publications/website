using Publications.API.DTOs;
using Publications.API.Models;
using Publications.API.Repositories.Shared;
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
        string searchTerm = paginationSearchDTO.SearchTerm;
        
        var query = new RedisQuery("publication-idx")
            .Where(nameof(Publication.Title).Prefix(searchTerm))
            .Or(nameof(Publication.Title).Search(searchTerm))
            .Or(nameof(Publication.Abstract).Prefix(searchTerm))
            .Or(nameof(Publication.Abstract).Search(searchTerm))
            .Or($"{nameof(Publication.Publisher)}_{nameof(Publisher.Name)}".Prefix(searchTerm))
            .Or($"{nameof(Publication.Publisher)}_{nameof(Publisher.Name)}".Search(searchTerm))
            .Or($"{nameof(Publication.Authors)}_{nameof(Author.Name)}".Prefix(searchTerm))
            .Or($"{nameof(Publication.Authors)}_{nameof(Author.Name)}".Search(searchTerm))
            .Filter(paginationSearchDTO.Filter)
            .Build();
        
        Task<SearchResponse<Publication>> matchedCountTask = _redisConnectionProvider.Connection
            .SearchAsync<Publication>(query);

        query.Limit(paginationSearchDTO.PageSize, paginationSearchDTO.Page);
        Task<SearchResponse<Publication>> paginatedPublicationsTask = _redisConnectionProvider.Connection
            .SearchAsync<Publication>(query);
        
        await Task.WhenAll(matchedCountTask, paginatedPublicationsTask);
        
        IReadOnlyCollection<Publication> publications = (await paginatedPublicationsTask)
            .Documents.Values.ToList().AsReadOnly();
        
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