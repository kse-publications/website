using Microsoft.Extensions.Options;
using Notion.Client;
using Publications.Application.Repositories;
using Publications.Domain.Collections;
using Publications.Domain.Publications;
using Publications.Domain.Shared.Slugs;
using Publications.Infrastructure.Source.Models;

namespace Publications.Infrastructure.Source;

public class NotionRepository: ISourceRepository
{
    private readonly INotionClient _notionClient;
    private readonly IWordsService _wordsService;
    private readonly NotionDatabaseOptions _databaseOptions;

    private IReadOnlyCollection<NotionPublisher>? _publishers;
    private IReadOnlyCollection<NotionAuthor>? _authors;
    private IReadOnlyCollection<NotionPublication>? _publications;
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
        return (await GetNotionPublicationsAsync())
            .Select(p => p.ToPublication())
            .ToList()
            .AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Collection>> GetCollectionsAsync()
    {
        return (await GetNotionCollectionsAsync())
            .Select(c => c.ToCollection()).ToList();
    }

    public async Task<(IReadOnlyCollection<Publication>, IReadOnlyCollection<Collection>)> GetPublicationsAndCollectionsAsync()
    {
        var publications = await GetNotionPublicationsAsync();
        var collections = await GetNotionCollectionsAsync();
        
        JoinPublicationsCollections(publications, collections);
        
        return (publications.Select(p => p.ToPublication()).ToList().AsReadOnly(),
            collections.Select(c => c.ToCollection()).ToList().AsReadOnly());
    }

    public async Task<IReadOnlyCollection<Author>> GetAuthorsAsync()
    {
        return (await GetNotionAuthorsAsync())
            .Select(a => a.ToAuthor())
            .ToList()
            .AsReadOnly();
    }
    
    public async Task<IReadOnlyCollection<Publisher>> GetPublishersAsync()
    {
        return await GetNotionPublishersAsync();
    }
    
    private async Task<IReadOnlyCollection<NotionPublication>> GetNotionPublicationsAsync()
    {
        if (_publications is not null)
            return _publications;
        
        var authors = (await GetNotionAuthorsAsync()).ToList();
        var publishers = (await GetNotionPublishersAsync()).ToList();
        
        List<Page> publicationsPages = await GetAllPagesAsync(_databaseOptions.PublicationsDbId);

        _publications = publicationsPages
            .Select(page => NotionPublication
                .MapFromPage(page, _wordsService)?
                .JoinAuthors(page, authors)
                .JoinPublisher(page, publishers))
            .Where(publication => publication is not null)
            .ToList()
            .AsReadOnly()!;
        
        return _publications;
    }

    private async Task<IReadOnlyCollection<NotionCollection>> GetNotionCollectionsAsync()
    {
        if (_collections is not null)
            return _collections;
        
        List<Page> collectionsPages = await GetAllPagesAsync(_databaseOptions.CollectionsDbId);
        
        _collections = collectionsPages
            .Select(page => NotionCollection.MapFromPage(page, _wordsService))
            .Where(collection => collection is not null)
            .ToList()
            .AsReadOnly()!;
        
        return _collections;
    }

    private async Task<IReadOnlyCollection<NotionAuthor>> GetNotionAuthorsAsync()
    {
        if (_authors is not null)
            return _authors;
        
        List<Page> authorsPages = await GetAllPagesAsync(_databaseOptions.AuthorsDbId);
        
        _authors = authorsPages
            .Select(page => NotionAuthor.MapFromPage(page))
            .Where(author => author is not null)
            .ToList()
            .AsReadOnly()!;
        
        return _authors;
    }
    
    private async Task<IReadOnlyCollection<NotionPublisher>> GetNotionPublishersAsync()
    {
        if (_publishers is not null)
            return _publishers;
        
        List<Page> publishersPages = await GetAllPagesAsync(_databaseOptions.PublishersDbId);
        
        _publishers = publishersPages
            .Select(page => NotionPublisher.MapFromPage(page))
            .Where(publisher => publisher is not null)
            .ToList()
            .AsReadOnly()!;
        
        return _publishers;
    }
    
    private static void JoinPublicationsCollections(
        IEnumerable<NotionPublication> publications, 
        IEnumerable<NotionCollection> collections)
    {
        Dictionary<string, NotionPublication> publicationsDictionary = publications
            .ToDictionary(p => p.GetNotionId());

        foreach (var collection in collections)
        {
            foreach (var relationId in collection.GetPublicationsRelation())
            {
                if (!publicationsDictionary.TryGetValue(relationId.Id, out NotionPublication? publication)) 
                    continue;
                
                publication.AddCollection(collection);
                collection.AddPublicationId(publication.Id);
            }
        }
    }
    
    private async Task<List<Page>> GetAllPagesAsync(string databaseId)
    {
        PaginatedList<Page> currentPage = await _notionClient.Databases.QueryAsync(
            databaseId, new DatabasesQueryParameters
            {
                PageSize = 100
            });
        
        List<Page> pages = [..currentPage.Results];

        while (currentPage.HasMore)
        {
            currentPage = await _notionClient.Databases.QueryAsync(
                databaseId, new DatabasesQueryParameters 
                {
                    PageSize = 100,
                    StartCursor = currentPage.NextCursor
                });
            
            pages.AddRange(currentPage.Results);
        }
        
        return pages;
    }
}