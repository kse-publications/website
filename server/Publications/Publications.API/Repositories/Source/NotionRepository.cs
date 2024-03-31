using Microsoft.Extensions.Options;
using Notion.Client;
using Publications.API.DTOs;
using Publications.API.Models;

namespace Publications.API.Repositories.Source;

public class NotionRepository: ISourceRepository
{
    private readonly INotionClient _notionClient;
    private readonly NotionDatabaseOptions _databaseOptions;

    public NotionRepository(
        INotionClient notionClient, 
        IOptions<NotionDatabaseOptions> databaseOptions)
    {
        _notionClient = notionClient;
        _databaseOptions = databaseOptions.Value;
    }

    public async Task<IReadOnlyCollection<Publication>> GetPublicationsAsync()
    {
        var authors = await GetAuthorsAsync();
        var publishers = await GetPublishersAsync();
        
        PaginatedList<Page> notionPublications = await _notionClient.Databases.QueryAsync(
            _databaseOptions.PublicationsDbId, new DatabasesQueryParameters());
        
        return notionPublications.Results
            .Select(page => new Publication
            {
                Id = (int)((UniqueIdPropertyValue)page.Properties["ID"]).UniqueId.Number!.Value,
                NotionId = Guid.Parse(page.Id),
                Title = ((TitlePropertyValue)page.Properties["Name"]).Title[0].PlainText,
                Type = ((SelectPropertyValue)page.Properties["Type"]).Select.Name,
                Year = (int)((NumberPropertyValue)page.Properties["Year"]).Number!.Value,
                Link = ((UrlPropertyValue)page.Properties["Link"]).Url,
                
                Authors = ((RelationPropertyValue)page.Properties["Authors"]).Relation
                    .Select(r => authors.FirstOrDefault(a => a.NotionId == Guid.Parse(r.Id)))
                    .ToArray()!,
                
                Publisher = publishers.FirstOrDefault(p => p.NotionId == Guid.Parse(
                    ((RelationPropertyValue)page.Properties["Publisher"]).Relation[0].Id))!,
                
                Keywords = ((RichTextPropertyValue)page.Properties["Keywords"]).RichText
                    .SelectMany(r => r.PlainText.Split(',', 
                        StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                    .ToArray(),
                
                Abstract = ((RichTextPropertyValue)page.Properties["Abstract"])
                    .RichText.Select(r => r.PlainText).FirstOrDefault()!,
                LastModified = page.LastEditedTime
            }.UpdateSlug())
            .ToList()
            .AsReadOnly();
    }
    
    public async Task<IReadOnlyCollection<Author>> GetAuthorsAsync()
    {
        PaginatedList<Page> notionAuthors = await _notionClient.Databases.QueryAsync(
            _databaseOptions.AuthorsDbId, new DatabasesQueryParameters());
        
        return notionAuthors.Results
            .Select(page => new Author 
            { 
                Id = (int)((UniqueIdPropertyValue)page.Properties["ID"]).UniqueId.Number!.Value,
                NotionId = Guid.Parse(page.Id),
                Name = ((TitlePropertyValue)page.Properties["Name"]).Title[0].PlainText,
                ProfileLink = ((UrlPropertyValue)page.Properties["Profile link"]).Url 
            }.UpdateSlug())
            .ToList()
            .AsReadOnly();
    }
    
    public async Task<IReadOnlyCollection<Publisher>> GetPublishersAsync()
    {
        PaginatedList<Page> notionPublishers = await _notionClient.Databases.QueryAsync(
            _databaseOptions.PublishersDbId, new DatabasesQueryParameters());
        
        return notionPublishers.Results
            .Select(page => new Publisher 
            { 
                Id = (int)((UniqueIdPropertyValue)page.Properties["ID"]).UniqueId.Number!.Value,
                NotionId = Guid.Parse(page.Id),
                Name = ((TitlePropertyValue)page.Properties["Name"]).Title[0].PlainText
            }.UpdateSlug())
            .ToList()
            .AsReadOnly();
    }
}