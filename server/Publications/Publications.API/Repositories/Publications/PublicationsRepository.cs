using Publications.API.DTOs;
using Publications.API.Models;
using Publications.API.Repositories.Shared;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Searching;
using Redis.OM.Searching.Query;

namespace Publications.API.Repositories.Publications;

public class PublicationsRepository: EntityRepository<Publication>, IPublicationsRepository
{
    private readonly IRedisConnectionProvider _redisConnectionProvider;
    private readonly IRedisCollection<Publication> _publications;

    public PublicationsRepository(IRedisConnectionProvider redisConnectionProvider)
        : base(redisConnectionProvider)
    {
        _redisConnectionProvider = redisConnectionProvider;
        _publications = redisConnectionProvider.RedisCollection<Publication>();
    }

    public async Task<PaginatedCollection<Publication>> GetAllAsync(
        PaginationFilterDTO paginationDTO, CancellationToken cancellationToken = default)
    {
        RedisQuery query = new RedisQuery("publication-idx")
            .Filter(paginationDTO)
            .Sort(nameof(Publication.Year), SortDirection.Descending);
        
        Task<SearchResponse<Publication>> matchedCountTask = _redisConnectionProvider.Connection
            .SearchAsync<Publication>(query);
        
        query.Limit(paginationDTO.PageSize, paginationDTO.Page);
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

    public async Task<PaginatedCollection<Publication>> GetBySearchAsync(
        PaginationFilterSearchDTO paginationSearchDTO,
        CancellationToken cancellationToken = default)
    {
        string searchTerm = paginationSearchDTO.SearchTerm;
        
        RedisQuery query = new RedisQuery("publication-idx")
            .Where(nameof(Publication.Title).Prefix(searchTerm))
            .Or(nameof(Publication.Title).Search(searchTerm))
            .Or(nameof(Publication.Abstract).Prefix(searchTerm))
            .Or(nameof(Publication.Abstract).Search(searchTerm))
            .Or($"{nameof(Publication.Publisher)}_{nameof(Publisher.Name)}".Prefix(searchTerm))
            .Or($"{nameof(Publication.Publisher)}_{nameof(Publisher.Name)}".Search(searchTerm))
            .Or($"{nameof(Publication.Authors)}_{nameof(Author.Name)}".Prefix(searchTerm))
            .Or($"{nameof(Publication.Authors)}_{nameof(Author.Name)}".Search(searchTerm))
            .Build()
            .Filter(paginationSearchDTO);
        
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
}