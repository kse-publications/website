using System.Text.Json;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Aggregation;
using Publications.Application.DTOs.Request;
using Publications.Application.DTOs.Response;
using Publications.Application.Repositories;
using Publications.Domain.Collections;
using Publications.Domain.Publications;
using Publications.Domain.Shared;
using Publications.Infrastructure.Shared;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Searching;
using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Publications.Infrastructure.Publications;

public class PublicationsRepository: EntityRepository<Publication>, IPublicationsRepository
{
    private readonly IDatabase _db;
    private readonly IRedisCollection<Publication> _publications;
    private readonly JsonSerializerOptions _jsonOptions;
    
    public PublicationsRepository(
        IConnectionMultiplexer connectionMultiplexer,
        IRedisConnectionProvider connectionProvider,
        JsonSerializerOptions jsonOptions) : base(connectionProvider)
    {
        _jsonOptions = jsonOptions;
        _db = connectionMultiplexer.GetDatabase();
        _publications = connectionProvider.RedisCollection<Publication>();
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetAllAsync(
        FilterDTO filterDTO, PaginationDTO paginationDTO,
        CancellationToken cancellationToken = default)
    {
        SearchCommands ft = _db.FT();
        SearchQuery query = SearchQuery.CreateWithFilter(filterDTO.GetParsedFilters());
        
        AggregationResult aggregationResult = await ft.AggregateAsync(Entity.IndexName<Publication>(),
            new AggregationRequest(query.Build())
                .Load(
                    new FieldName(nameof(Publication.Slug)),
                    new FieldName(nameof(Publication.Title)),
                    new FieldName(nameof(Publication.Type)),
                    new FieldName(nameof(Publication.Year)),
                    new FieldName(AuthorsName),
                    new FieldName(PublisherName))
                .SortBy(
                    new SortedField($"@{nameof(Publication.Year)}", SortedField.SortOrder.DESC),
                    new SortedField($"@{nameof(Publication.Id)}", SortedField.SortOrder.DESC))
                .Limit(paginationDTO.GetOffset(), paginationDTO.PageSize)
                .Dialect(3));
        
        IReadOnlyCollection<PublicationSummary> publications = 
            MapToPublicationSummaries(aggregationResult.GetResults())
                .AsReadOnly();
        
        return new PaginatedCollection<PublicationSummary>(
            Items: publications,
            TotalCount: (int)aggregationResult.TotalResults,
            ResultCount: publications.Count);
    }

    public async Task<IReadOnlyCollection<SyncEntityMetadata>> GetAllSyncMetadataAsync(
        CancellationToken cancellationToken = default)
    {
        return (await _publications.Select(e => new SyncEntityMetadata
        {
            Id = e.Id,
            LastSynchronizedAt = e.LastSynchronizedAt,
        }).ToListAsync()).AsReadOnly();
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        FilterDTO filterDTO, PaginationDTO paginationDTO, SearchDTO searchDTO,
        CancellationToken cancellationToken = default)
    {
        SearchCommands ft = _db.FT();

        SearchQuery query = SearchQuery.CreateWithSearch(searchDTO.SearchTerm)
            .Filter(filterDTO.GetParsedFilters());
        
        var searchResult = await ft.SearchAsync(Entity.IndexName<Publication>(), 
            new Query(query.Build())
                .LimitFields(Publication.GetSearchableFields())
                .Limit(paginationDTO.GetOffset(), paginationDTO.PageSize)
                .Dialect(3));

        var publications = MapToPublicationSummaries(searchResult);
        
        return new PaginatedCollection<PublicationSummary>(
            Items: publications.AsReadOnly(),
            TotalCount: (int)searchResult.TotalResults,
            ResultCount: publications.Count);
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetRelatedByAuthorsAsync(
        int currentPublicationId, PaginationDTO paginationDto, AuthorFilterDTO authorFilterDto, 
        CancellationToken cancellationToken = default)
    {
        SearchCommands ft = _db.FT();
        
        SearchField idSearchField = new(nameof(Publication.Id));
        SearchField authorsIdSearchField = new(AuthorId);
        var query = SearchQuery.Where(idSearchField.NotEqualTo(currentPublicationId));

        var authorsQuery = SearchQuery.MatchAll();
        foreach (int id in authorFilterDto.GetParsedAuthorsId())
        {
            authorsQuery.Or(authorsIdSearchField.EqualTo(id));
        }
        query.And(authorsQuery.Build());
        
        SearchResult searchResult = await ft.SearchAsync(Entity.IndexName<Publication>(), 
            new Query(query.Build())
                .SetSortBy(nameof(Publication.Views), ascending: false)
                .Limit(paginationDto.GetOffset(), paginationDto.PageSize)
                .Dialect(3));

        var publications = MapToPublicationSummaries(searchResult);
        
        return new PaginatedCollection<PublicationSummary>(
            Items: publications.AsReadOnly(),
            TotalCount: (int)searchResult.TotalResults,
            ResultCount: publications.Count);
    }

    public async Task<IReadOnlyCollection<PublicationSummary>> GetSimilarAsync(
        int currentPublicationId, 
        CancellationToken cancellationToken = default)
    {
        Publication? currentPublication = await GetByIdAsync(currentPublicationId, cancellationToken);
        if (currentPublication is null)
        {
            return Array.Empty<PublicationSummary>().AsReadOnly();
        }

        var ft = _db.FT();
        string idFilter = new SearchField(nameof(Publication.Id)).NotEqualTo(currentPublication.Id);
        const double radius = 0.35;
        
        var searchResult = await ft.SearchAsync(Entity.IndexName<Publication>(),
            new Query($"({idFilter}) @{SimilarityVector}:[VECTOR_RANGE {radius} $vec_param]=>{{$yield_distance_as: dist}}")
                .AddParam("vec_param", currentPublication.SimilarityVector.Embedding!)
                .SetSortBy("dist", ascending: true)
                .Dialect(3));

        return MapToPublicationSummaries(searchResult).AsReadOnly();
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetFromCollectionAsync(
        int collectionId,
        PaginationDTO paginationDTO,
        CancellationToken cancellationToken = default)
    {
        SearchCommands ft = _db.FT();
        string query = new SearchField(CollectionId).EqualTo(collectionId);
        
        SearchResult searchResult = await ft.SearchAsync(Entity.IndexName<Publication>(),
            new Query(query)
                .SetSortBy(nameof(Publication.Views), ascending: false)
                .Limit(paginationDTO.GetOffset(), paginationDTO.PageSize)
                .Dialect(3));

        var publications = MapToPublicationSummaries(searchResult);
        
        return new PaginatedCollection<PublicationSummary>(
            Items: publications.AsReadOnly(),
            TotalCount: (int)searchResult.TotalResults,
            ResultCount: publications.Count);
    }

    public async Task<Publication[]> GetNonVectorizedAsync(
        CancellationToken cancellationToken = default)
    {
        return (await _publications.Where(p => !p.Vectorized)
            .ToListAsync())
            .ToArray();
    }

    public async Task UpdatePropertyValueAsync(
        int publicationId,
        string propertyName, 
        string newValue,
        CancellationToken cancellationToken = default)
    {
        var json = _db.JSON();
        
        await json.SetAsync(
            key: Publication.GetKey(publicationId), 
            $"$.{propertyName}", newValue);
    }
    
    public async Task<PublicationSummary[]> GetTopPublicationsByRecentViews(
        int count = 4, 
        CancellationToken cancellationToken = default)
    {
        var topPublications = await _publications
            .OrderByDescending(pub => pub.RecentViews) 
            .Take(count)  
            .ToListAsync();  

        return topPublications
            .Select(PublicationSummary.FromPublication)
            .ToArray();
    }

    private static List<PublicationSummary> MapToPublicationSummaries(
        IEnumerable<Dictionary<string, RedisValue>> aggregationResults)
    {
        return aggregationResults.Select(result => new PublicationSummary
        {
            Slug = result[nameof(Publication.Slug)]!,
            Title = JsonSerializer.Deserialize<string[]>(result[nameof(Publication.Title)]!)!.First(),
            Type = result.TryGetValue(nameof(Publication.Type), out var typeValue) ? typeValue! : string.Empty,
            Year = result.TryGetValue(nameof(Publication.Year), out var yearValue) ? (int)yearValue! : 0,
            Authors = JsonSerializer
                .Deserialize<string[]>(result
                    .TryGetValue(AuthorsName, out var authorsValue) ? authorsValue! : "[]")
                ?.ToArray() ?? [],
            Publisher = JsonSerializer
                .Deserialize<string[]>(result
                    .TryGetValue(PublisherName, out var publisherValue) ? publisherValue! : "[]")
                ?.First() ?? string.Empty
        }).ToList();
    }

    private List<PublicationSummary> MapToPublicationSummaries(SearchResult result)
    {
        return result
            .ToJson()
            .Select(json => PublicationSummary.FromPublication(JsonSerializer
                    .Deserialize<Publication[]>(json, _jsonOptions)!.First()))
            .ToList();
    }

    private static string AuthorsName => 
        $"{nameof(Publication.Authors)}_{nameof(Author.Name)}";
    
    private static string PublisherName =>
        $"{nameof(Publication.Publisher)}_{nameof(Publisher.Name)}";
    
    private static string AuthorId =>
        $"{nameof(Publication.Authors)}_{nameof(Author.Id)}";
    
    private static string CollectionId =>
        $"{nameof(Publication.Collections)}_{nameof(Collection.Id)}";
    
    private static string SimilarityVector =>
        $"{nameof(Publication.SimilarityVector)}";
}