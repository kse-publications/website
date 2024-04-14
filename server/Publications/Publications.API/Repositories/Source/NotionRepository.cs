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
        var authors = (await GetAuthorsAsync()).ToList();
        var publishers = (await GetPublishersAsync()).ToList();
        
        PaginatedList<Page> notionPublications = await _notionClient.Databases.QueryAsync(
            _databaseOptions.PublicationsDbId, new DatabasesQueryParameters());
        
        return notionPublications.Results
            .Where(IsNotionPublicationValid)
            .Select(page => MapPublicationFromPage(page, authors, publishers))
            .ToList()
            .AsReadOnly();
    }

    private static Publication MapPublicationFromPage(
        Page page, ICollection<Author> authors, ICollection<Publisher> publishers)
    {
        return new Publication
        {
            Id = (int)((UniqueIdPropertyValue)page.Properties["ID"]).UniqueId.Number!.Value,
            NotionId = Guid.Parse(page.Id),
            Title = ((TitlePropertyValue)page.Properties["Name"]).Title[0].PlainText,
            Type = ((SelectPropertyValue)page.Properties["Type"]).Select.Name,
            Language = ((SelectPropertyValue)page.Properties["Language"]).Select?.Name ?? string.Empty,
            Year = (int)((NumberPropertyValue)page.Properties["Year"]).Number!.Value,
            Link = ((UrlPropertyValue)page.Properties["Link"]).Url,

            Authors = ((RelationPropertyValue)page.Properties["Authors"]).Relation
                .Select(r => authors.FirstOrDefault(a => a.NotionId == Guid.Parse(r.Id)))
                .Where(a => a is not null)
                .ToArray()!,

            Publisher =
                ((RelationPropertyValue)page.Properties["Publisher"])?.Relation?.FirstOrDefault()?.Id is not null
                    ? publishers.FirstOrDefault(p => p.NotionId == Guid.Parse(
                        ((RelationPropertyValue)page.Properties["Publisher"]).Relation[0].Id))!
                    : null,

            Keywords = ((RichTextPropertyValue)page.Properties["Keywords"]).RichText
                .SelectMany(r => r.PlainText.Split(',',
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                .ToArray(),

            Abstract = ((RichTextPropertyValue)page.Properties["Abstract"])
                .RichText.Select(r => r.PlainText).FirstOrDefault()!,

            LastModified = page.LastEditedTime
        }.UpdateSlug();
    }
    
    
    public async Task<IReadOnlyCollection<Author>> GetAuthorsAsync()
    {
        PaginatedList<Page> notionAuthors = await _notionClient.Databases.QueryAsync(
            _databaseOptions.AuthorsDbId, new DatabasesQueryParameters());
        
        return notionAuthors.Results
            .Where(IsNotionAuthorValid)
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
            .Where(IsNotionPublisherValid)
            .Select(page => new Publisher 
            { 
                Id = (int)((UniqueIdPropertyValue)page.Properties["ID"]).UniqueId.Number!.Value,
                NotionId = Guid.Parse(page.Id),
                Name = ((TitlePropertyValue)page.Properties["Name"]).Title[0].PlainText
            }.UpdateSlug())
            .ToList()
            .AsReadOnly();
    }

    private static bool IsNotionPublicationValid(Page publicationPage)
    {
        if (publicationPage.Properties["ID"] is not UniqueIdPropertyValue id ||
            publicationPage.Properties["Name"] is not TitlePropertyValue title ||
            publicationPage.Properties["Type"] is not SelectPropertyValue type ||
            publicationPage.Properties["Year"] is not NumberPropertyValue year ||
            publicationPage.Properties["Abstract"] is not RichTextPropertyValue abstractValue ||
            publicationPage.Properties["Visible"] is not CheckboxPropertyValue visible)
            return false;
    
        return id.UniqueId.Number.HasValue &&
               title.Title.Count > 0 &&
               title.Title[0].PlainText.Length > 0 &&
               type.Select?.Name.Length > 0 &&
               year.Number.HasValue &&
               abstractValue.RichText.Count > 0 &&
               abstractValue.RichText[0].PlainText.Length > 0 &&
               visible.Checkbox;
    }
    
    private static bool IsNotionAuthorValid(Page authorPage)
    {
        if (authorPage.Properties["ID"] is not UniqueIdPropertyValue id ||
            authorPage.Properties["Name"] is not TitlePropertyValue name)
            return false;
    
        return id.UniqueId.Number.HasValue && 
               name.Title.Count > 0 &&
               name.Title[0].PlainText.Length > 0;
    }
    
    private static bool IsNotionPublisherValid(Page publisherPage)
    {
        if (publisherPage.Properties["ID"] is not UniqueIdPropertyValue id ||
            publisherPage.Properties["Name"] is not TitlePropertyValue name)
            return false;
    
        return id.UniqueId.Number.HasValue && 
               name.Title.Count > 0 &&
               name.Title[0].PlainText.Length > 0;
    }
}