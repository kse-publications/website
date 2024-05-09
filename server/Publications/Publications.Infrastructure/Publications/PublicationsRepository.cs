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
using Publications.Domain.Shared;
using Publications.Infrastructure.Shared;
using Publications.Infrastructure.Shared.Queries;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Searching;
using Redis.OM.Searching.Query;
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
        AggregationResult aggregationResult = await ft.AggregateAsync(Publication.IndexName,
            new AggregationRequest(FilterQuery("", paginationDTO))
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

    private static string FilterQuery(string query, FilterDTO filterDTO)
    {
        if (filterDTO.GetParsedFilters().Length == 0)
            return query.Length == 0 ? "*" : query;
        
        foreach (int[] filters in filterDTO.GetParsedFilters())
        {
            if (filters.Length == 0)
                continue;

            string filterGroupQuery = FilterQuery(filters.First());
            foreach (var filter in filters.Skip(1))
            {
                filterGroupQuery = Queries.Either(
                    filterGroupQuery, FilterQuery(filter));
            }

            query = Queries.Both(query, filterGroupQuery);
        }
        
        return query;
    }
    
    private static string FilterQuery(int filterId) =>
        $"{nameof(Entity<Publication>.Filters)}_{nameof(Domain.Filters.Filter.Id)}".EqualTo(filterId);
    
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
    
    public async Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        PaginationFilterSearchDTO paginationSearchDTO,
        CancellationToken cancellationToken = default)
    {
        string searchTerm = paginationSearchDTO.SearchTerm;
        
        RedisQuery query = new RedisQuery(Publication.IndexName)
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
        
        return await GetPaginatedPublicationsAsync(query, paginationSearchDTO);
    }

    public override async Task InsertOrUpdateAsync(
        IEnumerable<Publication> entities,
        CancellationToken cancellationToken = default)
    {
        await _publications.DeleteAsync(await _publications.ToListAsync());
        await _publications.InsertAsync(entities);
    }
    
    private async Task<PaginatedCollection<PublicationSummary>> GetPaginatedPublicationsAsync(
        RedisQuery query, PaginationFilterDTO paginationDTO)
    {
        Task<SearchResponse<Publication>> matchedCountTask = _redisConnectionProvider.Connection
            .SearchAsync<Publication>(query);
        
        query.Limit(paginationDTO.PageSize, paginationDTO.Page);
        Task<SearchResponse<Publication>> paginatedPublicationsTask = _redisConnectionProvider.Connection
            .SearchAsync<Publication>(query);
        
        await Task.WhenAll(matchedCountTask, paginatedPublicationsTask);
        
        IReadOnlyCollection<Publication> publications = (await paginatedPublicationsTask)
            .Documents.Values.ToList().AsReadOnly();
        
        return new PaginatedCollection<PublicationSummary>(
            Items: publications.Select(PublicationSummary.FromPublication).ToList().AsReadOnly(),
            ResultCount: publications.Count,
            TotalCount: (int)(await matchedCountTask).DocumentCount);
    }
}

internal class MySearchCommands: SearchCommandsAsync
{
    private readonly IDatabaseAsync _db;
    public MySearchCommands(IDatabaseAsync db) : base(db)
    {
        _db = db;
    }
    
    public new async Task<AggregationResult> AggregateAsync(string index, AggregationRequest query)
    {
        if (!query.dialect.HasValue && this.defaultDialect.HasValue)
            query.Dialect(this.defaultDialect.Value);
        RedisResult result = await this._db.ExecuteAsync(SearchCommandBuilder.Aggregate(index, query));

        return null;
    }
}