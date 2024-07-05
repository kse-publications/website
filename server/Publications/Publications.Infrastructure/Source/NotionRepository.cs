using Microsoft.Extensions.Options;
using Notion.Client;
using Publications.Application.Repositories;
using Publications.Domain.Collections;
using Publications.Domain.Publications;
using Publications.Infrastructure.Source.Models;

namespace Publications.Infrastructure.Source;

public class NotionRepository: ISourceRepository
{
    private readonly INotionClient _notionClient;
    private readonly NotionDatabaseOptions _databaseOptions;

    private NotionPublisher[]? _publishers;
    private NotionAuthor[]? _authors;
    private NotionPublication[]? _publications;
    private NotionCollection[]? _collections;
    
    public NotionRepository(
        INotionClient notionClient, 
        IOptions<NotionDatabaseOptions> databaseOptions)
    {
        _notionClient = notionClient;
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
            .Select(c => c.ToCollection())
            .ToList();
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
                .MapFromPage(page)?
                .JoinAuthors(page, authors)
                .JoinPublisher(page, publishers))
            .Where(publication => publication is not null)
            .ToArray()!;
        
        return _publications;
    }

    private async Task<NotionCollection[]> GetNotionCollectionsAsync()
    {
        if (_collections is not null)
            return _collections;

        var publications = await GetNotionPublicationsAsync();
        List<Page> collectionsPages = await GetAllPagesAsync(_databaseOptions.CollectionsDbId);
        
        NotionCollection[] collections = collectionsPages
            .Select(page => NotionCollection.MapFromPage(page))
            .Where(collection => collection is not null)
            .ToArray()!;
        
        _collections = NotionCollection.JoinPublications(collections, publications);
        return _collections;
    }

    private async Task<NotionAuthor[]> GetNotionAuthorsAsync()
    {
        if (_authors is not null)
            return _authors;
        
        List<Page> authorsPages = await GetAllPagesAsync(_databaseOptions.AuthorsDbId);
        
        _authors = authorsPages
            .Select(page => NotionAuthor.MapFromPage(page))
            .Where(author => author is not null)
            .ToArray()!;
        
        return _authors;
    }
    
    private async Task<NotionPublisher[]> GetNotionPublishersAsync()
    {
        if (_publishers is not null)
            return _publishers;
        
        List<Page> publishersPages = await GetAllPagesAsync(_databaseOptions.PublishersDbId);
        
        _publishers = publishersPages
            .Select(page => NotionPublisher.MapFromPage(page))
            .Where(publisher => publisher is not null)
            .ToArray()!;
        
        return _publishers;
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