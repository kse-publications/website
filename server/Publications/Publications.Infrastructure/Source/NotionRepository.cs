using Microsoft.Extensions.Options;
using Notion.Client;
using Publications.Application.Repositories;
using Publications.Domain.Authors;
using Publications.Domain.Publications;
using Publications.Domain.Publishers;
using Publications.Domain.Shared.Slugs;

namespace Publications.Infrastructure.Source;

public class NotionRepository: ISourceRepository
{
    private readonly INotionClient _notionClient;
    private readonly IWordsService _wordsService;
    private readonly NotionDatabaseOptions _databaseOptions;

    private IReadOnlyCollection<NotionPublisher>? _publishers;
    private IReadOnlyCollection<NotionAuthor>? _authors;
    private IReadOnlyCollection<Publication>? _publications;
    private IReadOnlyCollection<NotionCollection>? _collections;
    
    public NotionRepository(
        INotionClient notionClient, 
        IWordsService wordsService,
        IOptions<NotionDatabaseOptions> databaseOptions)
    {
        _notionClient = notionClient;
        _wordsService = wordsService;
        _databaseOptions = databaseOptions.Value;
    }

    public async Task<IReadOnlyCollection<Publication>> GetPublicationsAsync()
    {
        if (_publications is not null)
            return _publications;
        
        var authors = (await GetNotionAuthorsAsync()).ToList();
        var publishers = (await GetNotionPublishersAsync()).ToList();
        var collections = (await GetNotionCollectionsAsync()).ToList();
        
        PaginatedList<Page> notionPublications = await _notionClient.Databases.QueryAsync(
            _databaseOptions.PublicationsDbId, new DatabasesQueryParameters());
        
        _publications = notionPublications.Results
            .Where(page => page.IsValidPublication())
            .Select(page => 
                page.MapToPublication(_wordsService)
                    .LinkAuthors(page, authors)
                    .LinkPublisher(page, publishers))
            .LinkCollections(collections)
            .ToList()
            .AsReadOnly();
        
        return _publications;
    }

    public async Task<IReadOnlyCollection<Collection>> GetCollectionsAsync()
    {
        return (await GetNotionCollectionsAsync())
            .Select(c => c.ToCollection()).ToList();
    }
    
    public async Task<IReadOnlyCollection<Author>> GetAuthorsAsync()
    {
        return (await GetNotionAuthorsAsync())
            .Select(a => a.ToAuthor()).ToList();
    }
    
    public async Task<IReadOnlyCollection<Publisher>> GetPublishersAsync()
    {
        return (await GetNotionPublishersAsync())
            .Select(p => p.ToPublisher()).ToList();
    }

    private async Task<IReadOnlyCollection<NotionCollection>> GetNotionCollectionsAsync()
    {
        if (_collections is not null)
            return _collections;
        
        PaginatedList<Page> notionCollections = await _notionClient.Databases.QueryAsync(
            _databaseOptions.CollectionsDbId, new DatabasesQueryParameters());
        
        _collections = notionCollections.Results
            .Where(page => page.IsValidCollection())
            .Select(page => page.MapToCollection(_wordsService))
            .ToList()
            .AsReadOnly();
        
        return _collections;
    }

    private async Task<IReadOnlyCollection<NotionAuthor>> GetNotionAuthorsAsync()
    {
        if (_authors is not null)
            return _authors;
        
        PaginatedList<Page> notionAuthors = await _notionClient.Databases.QueryAsync(
            _databaseOptions.AuthorsDbId, new DatabasesQueryParameters());
        
        _authors = notionAuthors.Results
            .Where(page => page.IsValidAuthor())
            .Select(page => page.MapToAuthor(_wordsService))
            .ToList()
            .AsReadOnly();
        
        return _authors;
    }
    
    private async Task<IReadOnlyCollection<NotionPublisher>> GetNotionPublishersAsync()
    {
        if (_publishers is not null)
            return _publishers;
        
        PaginatedList<Page> notionPublishers = await _notionClient.Databases.QueryAsync(
            _databaseOptions.PublishersDbId, new DatabasesQueryParameters());
        
        _publishers = notionPublishers.Results
            .Where(page => page.IsValidPublisher())
            .Select(page => page.MapToPublisher(_wordsService))
            .ToList()
            .AsReadOnly();
        
        return _publishers;
    }
}