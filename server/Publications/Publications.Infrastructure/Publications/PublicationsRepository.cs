using System.Text.Json;
using Microsoft.Extensions.Logging;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Aggregation;
using Publications.Application;
using Publications.Application.DTOs;
using Publications.Application.Repositories;
using Publications.Domain.Authors;
using Publications.Domain.Publications;
using Publications.Domain.Publishers;
using Publications.Infrastructure.Shared;
using Redis.OM.Contracts;
using Redis.OM.Searching;
using StackExchange.Redis;

namespace Publications.Infrastructure.Publications;

public class PublicationsRepository: EntityRepository<Publication>, IPublicationsRepository
{
    private readonly IRedisConnectionProvider _redisConnectionProvider;
    private readonly IRedisCollection<Publication> _publications;
    private readonly ILogger<PublicationsRepository> _logger;
    private readonly IDatabase _db;

    public PublicationsRepository(
        IRedisConnectionProvider redisConnectionProvider, 
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<PublicationsRepository> logger) : base(redisConnectionProvider)
    {
        _redisConnectionProvider = redisConnectionProvider;
        _logger = logger;
        _publications = redisConnectionProvider.RedisCollection<Publication>();
        _db = connectionMultiplexer.GetDatabase();
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetAllAsync(
        PaginationFilterDTO paginationDTO, CancellationToken cancellationToken = default)
    {
        SearchCommands ft = _db.FT();
        SearchQuery query = SearchQuery.CreateWithFilter(paginationDTO);
        
        AggregationResult aggregationResult = await ft.AggregateAsync(Publication.IndexName,
            new AggregationRequest(query.Build())
                .Load(
                    new FieldName(nameof(Publication.Slug)),
                    new FieldName(nameof(Publication.Title)),
                    new FieldName(nameof(Publication.Type)),
                    new FieldName(nameof(Publication.Year)),
                    new FieldName($"{nameof(Publication.Authors)}_{nameof(Author.Name)}"),
                    new FieldName($"{nameof(Publication.Publisher)}_{nameof(Publisher.Name)}"))
                .SortBy(
                    new SortedField("@Year", SortedField.SortOrder.DESC),
                    new SortedField("@Id", SortedField.SortOrder.DESC))
                .Limit(
                    offset: paginationDTO.PageSize * (paginationDTO.Page - 1),
                    count: paginationDTO.PageSize)
                .Dialect(3));
        
        IReadOnlyCollection<PublicationSummary> publications = 
            MapToPublicationSummaries(aggregationResult.GetResults())
                .ToList()
                .AsReadOnly();
        
        return new PaginatedCollection<PublicationSummary>(
            Items: publications,
            TotalCount: (int)aggregationResult.TotalResults,
            ResultCount: publications.Count);
    }
    
    public async Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        PaginationFilterSearchDTO paginationSearchDTO,
        CancellationToken cancellationToken = default)
    {
        SearchCommands ft = _db.FT();
        string searchTerm = paginationSearchDTO.SearchTerm;
        
        SearchFieldName title = new(nameof(Publication.Title));
        SearchFieldName abstractField = new(nameof(Publication.Abstract));
        SearchFieldName publisherName = new($"{nameof(Publication.Publisher)}_{nameof(Publisher.Name)}");
        SearchFieldName authorsName = new($"{nameof(Publication.Authors)}_{nameof(Author.Name)}");

        SearchQuery query = SearchQuery
            .Where(title.Prefix(searchTerm))
            .Or(title.Search(searchTerm))
            .Or(abstractField.Prefix(searchTerm))
            .Or(abstractField.Search(searchTerm))
            .Or(publisherName.Prefix(searchTerm))
            .Or(publisherName.Search(searchTerm))
            .Or(authorsName.Prefix(searchTerm))
            .Or(authorsName.Search(searchTerm))
            .Filter(paginationSearchDTO);
        
        var searchResult = await ft.SearchAsync(Publication.IndexName, 
            new Query(query.Build())
                .Limit(
                    offset: paginationSearchDTO.PageSize * (paginationSearchDTO.Page - 1),
                    count: paginationSearchDTO.PageSize)
                .Dialect(3));
        
        IReadOnlyCollection<PublicationSummary> publications = searchResult
            .ToJson()
            .Select(json => PublicationSummary
                .FromPublication(JsonSerializer
                    .Deserialize<Publication[]>(json)!.First()))
            .ToList()
            .AsReadOnly();
        
        return new PaginatedCollection<PublicationSummary>(
            Items: publications,
            TotalCount: (int)searchResult.TotalResults,
            ResultCount: publications.Count);
    }

    public override async Task InsertOrUpdateAsync(
        IEnumerable<Publication> entities,
        CancellationToken cancellationToken = default)
    {
        await _publications.DeleteAsync(await _publications.ToListAsync());
        await _publications.InsertAsync(entities);
    }
    
    private static ICollection<PublicationSummary> MapToPublicationSummaries(
        IEnumerable<Dictionary<string, RedisValue>> aggregationResults)
    {
        return aggregationResults.Select(result => new PublicationSummary
        {
            Slug = result[nameof(Publication.Slug)]!,
            Title = JsonSerializer.Deserialize<string[]>(result[nameof(Publication.Title)]!)!.First(),
            Type = result[nameof(Publication.Type)]!,
            Year = (int)result[nameof(Publication.Year)]!,
            Authors = JsonSerializer.Deserialize<string[]>(result[$"{nameof(Publication.Authors)}_{nameof(Author.Name)}"]!)!,
            Publisher = JsonSerializer.Deserialize<string[]>(result[$"{nameof(Publication.Publisher)}_{nameof(Publisher.Name)}"]!)!.First()
        }).ToList();
    }
}