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
using Publications.Infrastructure.Services.DbConfiguration;
using Publications.Infrastructure.Shared;
using Redis.OM;
using Redis.OM.Searching;
using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Publications.Infrastructure.Publications;

public class PublicationsRepository: EntityRepository<Publication>, IPublicationsRepository
{
    private readonly string _publicationIndex = RedisIndexVersionInfo.GetIndexName(typeof(Publication));
    
    private readonly IDatabase _db;
    private readonly IRedisCollection<Publication> _publications;
    private readonly ICollectionsRepository _collectionsRepository;
    private readonly JsonSerializerOptions _jsonOptions;
    
    public PublicationsRepository(
        IConnectionMultiplexer connection,
        JsonSerializerOptions jsonOptions,
        ICollectionsRepository collectionsRepository) : base(connection)
    {
        _jsonOptions = jsonOptions;
        _collectionsRepository = collectionsRepository;
        _db = connection.GetDatabase();
        RedisConnectionProvider provider = new(connection);
        _publications = provider.RedisCollection<Publication>();
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetAllAsync(
        FilterDTO filterDTO, PaginationDTO paginationDTO,
        CancellationToken cancellationToken = default)
    {
        SearchCommands ft = _db.FT();
        SearchQuery query = SearchQuery.CreateWithFilter(filterDTO.GetParsedFilters());
        
        AggregationResult aggregationResult = await ft.AggregateAsync(_publicationIndex,
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

    public async Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        FilterDTO filterDTO, PaginationDTO paginationDTO, SearchDTO searchDTO,
        CancellationToken cancellationToken = default)
    {
        SearchCommands ft = _db.FT();

        SearchQuery query = SearchQuery.CreateWithSearch(searchDTO.SearchTerm)
            .Filter(filterDTO.GetParsedFilters());
        
        var searchResult = await ft.SearchAsync(_publicationIndex, 
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

    public async Task<Publication[]> GetAllMatchedByKeywordsAsync(
        string[] keywords, CancellationToken cancellationToken = default)
    {
        SearchCommands ft = _db.FT();
        SearchQuery query = SearchQuery.CreateWithSearch(keywords.First());
        
        foreach (string keyword in keywords.Skip(1))
        {
            query.Or(SearchQuery.CreateWithSearch(keyword).Build());
        }
        
        SearchResult searchResult = await ft.SearchAsync(_publicationIndex, 
            new Query(query.Build())
                .LimitFields(nameof(Publication.Title))
                .Limit(0, 10_000)
                .Dialect(3));

        return MapToPublication(searchResult).ToArray();
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
        
        SearchResult searchResult = await ft.SearchAsync(_publicationIndex, 
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
        if (currentPublication?.SimilarityVector?.Embedding is null)
        {
            return Array.Empty<PublicationSummary>().AsReadOnly();
        }

        var ft = _db.FT();
        string idFilter = new SearchField(nameof(Publication.Id)).NotEqualTo(currentPublication.Id);
        double radius = currentPublication.Language == "Ukrainian" ? 0.18 : 0.35;
        
        var searchResult = await ft.SearchAsync(_publicationIndex,
            new Query($"({idFilter}) @{SimilarityVector}:[VECTOR_RANGE {radius} $vec_param]=>{{$yield_distance_as: dist}}")
                .AddParam("vec_param", currentPublication.SimilarityVector.Embedding)
                .SetSortBy("dist", ascending: true)
                .Dialect(3));

        return MapToPublicationSummaries(searchResult).AsReadOnly();
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetFromCollectionAsync(
        int collectionId,
        PaginationDTO paginationDTO,
        CancellationToken cancellationToken = default)
    {
        Collection? collection = await _collectionsRepository.GetByIdAsync(collectionId, cancellationToken);
        if (collection is null)
        {
            return PaginatedCollection<PublicationSummary>.GetEmpty();
        }

        SearchCommands ft = _db.FT();
        SearchField idField = new(nameof(Publication.Id));
        SearchQuery query = SearchQuery.CreateWithSearch(collection.Keywords.First());
        foreach (string keyword in collection.Keywords.Skip(1))
        {
            query.Or(SearchQuery.CreateWithSearch(keyword).Build());
        }

        foreach (int id in collection.GetPublicationIds())
        {
            query.Or(idField.EqualTo(id));
        }
        
        foreach (int id in collection.IgnoredPublicationIds)
        {
            query.And(idField.NotEqualTo(id));
        }
        
        SearchResult searchResult = await ft.SearchAsync(_publicationIndex,
            new Query(query.Build())
                .LimitFields(nameof(Publication.Title))
                .Limit(paginationDTO.GetOffset(), paginationDTO.PageSize)
                .Dialect(3));

        var publications = MapToPublicationSummaries(searchResult);
        
        return new PaginatedCollection<PublicationSummary>(
            Items: publications.AsReadOnly(),
            TotalCount: (int)searchResult.TotalResults,
            ResultCount: publications.Count);
    }
    
    public async Task UpdateAsync(
        int id,
        string propertyName, 
        string newValue,
        CancellationToken cancellationToken = default)
    {
        var json = _db.JSON();
        
        await json.SetAsync(
            key: GetPublicationKey(id), 
            path: $"$.{propertyName}", 
            json: newValue);
    }
    
    public async Task DeleteAsync(
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default)
    {
        await DeleteAsync(ids, GetPublicationKey, cancellationToken);
    }
    
    public async Task<IReadOnlyCollection<SyncEntityMetadata>> GetAllSyncMetadataAsync()
    {
        return (await _publications.Select(e => new SyncEntityMetadata
        {
            Id = e.Id,
            LastSynchronizedAt = e.LastSynchronizedAt,
        }).ToListAsync()).AsReadOnly();
    }

    public async Task<Publication[]> GetNonVectorizedAsync()
    {
        return (await _publications.Where(p => !p.Vectorized)
            .ToListAsync())
            .ToArray();
    }
    
    // public async Task<Publication[]> GetWithNonPopulatedCollectionsAsync()
    // {
    //     return (await _publications.Where(p => !p.PopulatedCollections)
    //         .ToListAsync())
    //         .ToArray();
    // }
    
    public async Task<PublicationSummary[]> GetTopPublicationsByRecentViews(int count = 4)
    {
        var topPublications = await _publications
            .OrderByDescending(pub => pub.RecentViews) 
            .Take(count)  
            .ToListAsync();  

        return topPublications
            .Select(p => new PublicationSummary(p))
            .ToArray();
    }

    private static List<PublicationSummary> MapToPublicationSummaries(
        IEnumerable<Dictionary<string, RedisValue>> aggregationResults)
    {
        return aggregationResults.Select(result => new PublicationSummary(
        
            slug: result[nameof(Publication.Slug)]!,
            title: JsonSerializer.Deserialize<string[]>(result[nameof(Publication.Title)]!)!.First(),
            type: result.TryGetValue(nameof(Publication.Type), out var typeValue) ? typeValue! : string.Empty,
            year: result.TryGetValue(nameof(Publication.Year), out var yearValue) ? (int)yearValue! : 0,
            authors: JsonSerializer
                .Deserialize<string[]>(result
                    .TryGetValue(AuthorsName, out var authorsValue) ? authorsValue! : "[]")
                ?.ToArray() ?? [],
            publisher: JsonSerializer
                .Deserialize<string[]>(result
                    .TryGetValue(PublisherName, out var publisherValue) ? publisherValue! : "[]")
                ?.First() ?? string.Empty
        )).ToList();
    }

    private List<PublicationSummary> MapToPublicationSummaries(SearchResult result)
    {
        return result
            .ToJson()
            .Select(json => new PublicationSummary(JsonSerializer
                    .Deserialize<Publication[]>(json, _jsonOptions)!.First()))
            .ToList();
    }
    
    private List<Publication> MapToPublication(SearchResult result)
    {
        return result
            .ToJson()
            .Select(json => JsonSerializer
                .Deserialize<Publication[]>(json, _jsonOptions)!.First())
            .ToList();
    }

    private static string GetPublicationKey(int id) => $"publication:{id}";

    private static string AuthorsName => 
        $"{nameof(Publication.Authors)}_{nameof(Author.Name)}";
    
    private static string PublisherName =>
        $"{nameof(Publication.Publisher)}_{nameof(Publisher.Name)}";
    
    private static string AuthorId =>
        $"{nameof(Publication.Authors)}_{nameof(Author.Id)}";
    
    // private static string CollectionId =>
    //     $"{nameof(Publication.Collections)}_{nameof(Collection.Id)}";
    
    private static string SimilarityVector =>
        $"{nameof(Publication.SimilarityVector)}";
}