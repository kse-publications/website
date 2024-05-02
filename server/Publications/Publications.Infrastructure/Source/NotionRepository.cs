using Microsoft.Extensions.Options;
using Notion.Client;
using Publications.Application.Repositories;
using Publications.Domain.Authors;
using Publications.Domain.Publications;
using Publications.Domain.Publishers;
using Publications.Domain.Shared;

namespace Publications.Infrastructure.Source;

public class NotionRepository: ISourceRepository
{
    private readonly INotionClient _notionClient;
    private readonly IWordsService _wordsService;
    private readonly NotionDatabaseOptions _databaseOptions;

    private IReadOnlyCollection<Publisher>? _publishers;
    private IReadOnlyCollection<Author>? _authors;
    private IReadOnlyCollection<Publication>? _publications;

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
        //TODO: HIGH VOLUME ITERATION 
        if (_publications is not null)
            return _publications;
        
        var authors = (await GetAuthorsAsync()).ToList();
        var publishers = (await GetPublishersAsync()).ToList();
        
        PaginatedList<Page> notionPublications = await _notionClient.Databases.QueryAsync(
            _databaseOptions.PublicationsDbId, new DatabasesQueryParameters());
        
        _publications = notionPublications.Results
            .Where(page => page.IsValidPublication())
            .Select(page => page.MapToPublication(authors, publishers, _wordsService))
            .ToList()
            .AsReadOnly();
        
        return _publications;
    }
    
    public async Task<IReadOnlyCollection<Author>> GetAuthorsAsync()
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
    
    public async Task<IReadOnlyCollection<Publisher>> GetPublishersAsync()
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