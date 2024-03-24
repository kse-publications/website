using Microsoft.Extensions.Options;
using Notion.Client;
using Publications.API.Models;

namespace Publications.API.Notion;

public class NotionRepository
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

    public async Task<IReadOnlyCollection<Publication>> GetPublications()
    {
        var authors = await GetAuthors();
        var publishers = await GetPublishers();
        
        PaginatedList<Page> notionPublications = await _notionClient.Databases.QueryAsync(
            _databaseOptions.PublicationsDbId, new DatabasesQueryParameters());

        return notionPublications.Results
            .Select(page => new Publication
            {
                Id = Guid.Parse(page.Id),
                Title = ((TitlePropertyValue)page.Properties["Name"]).Title[0].PlainText,
                Type = ((SelectPropertyValue)page.Properties["Type"]).Select.Name,
                Year = (int)((NumberPropertyValue)page.Properties["Year"]).Number!.Value,
                Link = ((UrlPropertyValue)page.Properties["Link"]).Url,
                
                Authors = ((RelationPropertyValue)page.Properties["Authors"]).Relation
                    .Select(r => authors.FirstOrDefault(a => a.Id == Guid.Parse(r.Id)))
                    .ToArray(),
                
                Publisher = publishers.FirstOrDefault(p => p.Id == Guid.Parse(
                    ((RelationPropertyValue)page.Properties["Publisher"]).Relation[0].Id)),
                
                Keywords = ((RichTextPropertyValue)page.Properties["Keywords"])
                    .RichText.Select(r => r.PlainText).ToArray(),
                
                Abstract = ((RichTextPropertyValue)page.Properties["Abstract"])
                    .RichText.Select(r => r.PlainText).FirstOrDefault()!
            })
            .ToList()
            .AsReadOnly();
    }
    
    private async Task<IReadOnlyCollection<Author>> GetAuthors()
    {
        PaginatedList<Page> notionAuthors = await _notionClient.Databases.QueryAsync(
            _databaseOptions.AuthorsDbId, new DatabasesQueryParameters());
        
        return notionAuthors.Results
            .Select(page => new Author 
            { 
                Id = Guid.Parse(page.Id),
                Name = ((TitlePropertyValue)page.Properties["Name"]).Title[0].PlainText,
                ProfileLink = ((UrlPropertyValue)page.Properties["Profile link"]).Url 
            })
            .ToList()
            .AsReadOnly();
    }
    
    private async Task<IReadOnlyCollection<Publisher>> GetPublishers()
    {
        PaginatedList<Page> notionPublishers = await _notionClient.Databases.QueryAsync(
            _databaseOptions.PublishersDbId, new DatabasesQueryParameters());
        
        return notionPublishers.Results
            .Select(page => new Publisher 
            { 
                Id = Guid.Parse(page.Id),
                Name = ((TitlePropertyValue)page.Properties["Name"]).Title[0].PlainText
            })
            .ToList()
            .AsReadOnly();
    }
}