
using System.Text.Json;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Aggregation;
using Publications.Application;
using Publications.Application.DTOs;
using Publications.Application.Repositories;
using Publications.Domain.Collections;
using Publications.Domain.Publications;
using Publications.Infrastructure.Shared;
using Redis.OM;
using Redis.OM.Searching;
using StackExchange.Redis;

namespace Publications.Infrastructure.Publications;

public class PublicationsRepository: EntityRepository<Publication>, IPublicationsRepository
{
    private readonly IDatabase _db;
    private IPublicationsRepository _publicationsRepositoryImplementation;
    private readonly RedisCollection<Publication> _publications;

    public PublicationsRepository(IConnectionMultiplexer connectionMultiplexer) 
        : base(new RedisConnectionProvider(connectionMultiplexer))
    {
        _db = connectionMultiplexer.GetDatabase();
        // var provider = new RedisConnectionProvider(connectionMultiplexer); 
        // _publications = provider.RedisCollection<Publication>();
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetAllAsync(
        FilterDTO filterDTO, PaginationDTO paginationDTO,
        CancellationToken cancellationToken = default)
    {
        SearchCommands ft = _db.FT();
        SearchQuery query = SearchQuery.CreateWithFilter(filterDTO);
        
        AggregationResult aggregationResult = await ft.AggregateAsync(Publication.IndexName,
            new AggregationRequest(query.Build())
                .Load(
                    new FieldName(nameof(Publication.Slug)),
                    new FieldName(nameof(Publication.Title)),
                    new FieldName(nameof(Publication.Type)),
                    new FieldName(nameof(Publication.Year)),
                    new FieldName(PublicationAuthorsName),
                    new FieldName(PublicationPublisherName))
                .SortBy(
                    new SortedField($"@{nameof(Publication.Year)}", SortedField.SortOrder.DESC),
                    new SortedField($"@{nameof(Publication.Id)}", SortedField.SortOrder.DESC))
                .Paginate(paginationDTO.Page, paginationDTO.PageSize)
                .Dialect(3));
        
        IReadOnlyCollection<PublicationSummary> publications = 
            MapToPublicationSummaries(aggregationResult.GetResults())
                .AsReadOnly();
        
        return new PaginatedCollection<PublicationSummary>(
            Items: publications,
            TotalCount: (int)aggregationResult.TotalResults,
            ResultCount: publications.Count);
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        FilterDTO filterDTO, PaginationDTO paginationDTO, SearchDTO searchDTO,
        CancellationToken cancellationToken = default)
    {
        SearchCommands ft = _db.FT();
        
        SearchFieldName[] searchFields = Publication.GetSearchableFields()
            .Select(fieldName => new SearchFieldName(fieldName))
            .ToArray();

        SearchQuery query = SearchQuery
            .CreateWithSearch(searchDTO.SearchTerm, searchFields)
            .Filter(filterDTO);
        
        var searchResult = await ft.SearchAsync(Publication.IndexName, 
            new Query(query.Build())
                .Paginate(paginationDTO.Page, paginationDTO.PageSize)
                .Dialect(3));
        
        var publications = MapToPublicationSummaries(searchResult)
            .AsReadOnly();
        
        return new PaginatedCollection<PublicationSummary>(
            Items: publications,
            TotalCount: (int)searchResult.TotalResults,
            ResultCount: publications.Count);
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetRelatedByAuthorsAsync(
        int currentPublicationId, PaginationDTO paginationDto, AuthorFilterDTO authorFilterDto, 
        CancellationToken cancellationToken = default)
    {
        SearchCommands ft = _db.FT();
        int[] authorsIds = authorFilterDto.GetParsedAuthorsId();
        
        SearchFieldName idSearchField = new(nameof(Publication.Id));
        SearchFieldName authorsIdSearchField = new(PublicationAuthorsId);
        var query = SearchQuery.Where(
            idSearchField.NotEqualTo(currentPublicationId));

        var authorsQuery = SearchQuery.MatchAll();
        foreach (var id in authorsIds)
        {
            authorsQuery.Or(authorsIdSearchField.EqualTo(id));
        }
        query.And(authorsQuery.Build());
        
        SearchResult searchResult = await ft.SearchAsync(Publication.IndexName, 
            new Query(query.Build())
                .SetSortBy(nameof(Publication.Views), ascending: false)
                .Paginate(paginationDto.Page, paginationDto.PageSize)
                .Dialect(3));

        var publications = MapToPublicationSummaries(searchResult)
            .AsReadOnly();
        
        return new PaginatedCollection<PublicationSummary>(
            Items: publications,
            TotalCount: (int)searchResult.TotalResults,
            ResultCount: publications.Count);
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetFromCollectionAsync(
        int collectionId,
        PaginationDTO paginationDTO,
        CancellationToken cancellationToken = default)
    {
        SearchCommands ft = _db.FT();
        SearchQuery query = SearchQuery
            .Where(new SearchFieldName(PublicationCollectionsId)
                .EqualTo(collectionId));
        
        SearchResult searchResult = await ft.SearchAsync(Publication.IndexName,
            new Query(query.Build())
                .SetSortBy(nameof(Publication.Views), ascending: false)
                .Paginate(paginationDTO.Page, paginationDTO.PageSize)
                .Dialect(3));
        
        var publications = MapToPublicationSummaries(searchResult)
            .AsReadOnly();
        
        return new PaginatedCollection<PublicationSummary>(
            Items: publications,
            TotalCount: (int)searchResult.TotalResults,
            ResultCount: publications.Count);
    }

    private static List<PublicationSummary> MapToPublicationSummaries(
        IEnumerable<Dictionary<string, RedisValue>> aggregationResults)
    {
        return aggregationResults.Select(result => new PublicationSummary
        {
            Slug = result[nameof(Publication.Slug)]!,
            Title = JsonSerializer.Deserialize<string[]>(result[nameof(Publication.Title)]!)!.First(),
            Type = result.TryGetValue(nameof(Publication.Type), out var typeValue) 
                ? typeValue! : string.Empty,
            Year = result.TryGetValue(nameof(Publication.Year), out var yearValue) 
                ? (int)yearValue! : 0,
            Authors = JsonSerializer
                .Deserialize<string[]>(result
                    .TryGetValue(PublicationAuthorsName, out var authorsValue) ? authorsValue! : "[]")
                ?.ToArray() ?? [],
            Publisher = JsonSerializer
                .Deserialize<string[]>(result
                    .TryGetValue(PublicationPublisherName, out var publisherValue) ? publisherValue! : "[]")
                ?.First() ?? string.Empty
        }).ToList();
    }

    private static List<PublicationSummary> MapToPublicationSummaries(SearchResult result)
    {
        return result
            .ToJson()
            .Select(json => PublicationSummary
                .FromPublication(JsonSerializer
                    .Deserialize<Publication[]>(json)!.First()))
            .ToList();
    }
    
    // public async Task<PublicationSummary[]> GetTopPublicationsByRecentViews()
    // {
    //      var topPublications = await _publications
    //          .OrderByDescending(pub => pub.RecentViews)
    //          .Take(4)
    //        .ToListAsync();
    //
    //     return topPublications;
    // }

    private static string PublicationAuthorsName => 
        $"{nameof(Publication.Authors)}_{nameof(Author.Name)}";
    
    private static string PublicationPublisherName =>
        $"{nameof(Publication.Publisher)}_{nameof(Publisher.Name)}";
    
    private static string PublicationAuthorsId =>
        $"{nameof(Publication.Authors)}_{nameof(Author.Id)}";
    
    private static string PublicationCollectionsId =>
        $"{nameof(Publication.Collections)}_{nameof(Collection.Id)}";
}