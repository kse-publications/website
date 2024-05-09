using Notion.Client;
using Publications.Domain.Authors;
using Publications.Domain.Publications;
using Publications.Domain.Publishers;
using Publications.Domain.Shared;

namespace Publications.Infrastructure.Source;

public static class NotionModelsMapperExtensions
{
    public static Publisher MapToPublisher(this Page page, IWordsService wordsService)
    {
        return new Publisher
        {
            Id = (int)((UniqueIdPropertyValue)page.Properties["ID"]).UniqueId.Number!.Value,
            NotionId = Guid.Parse(page.Id),
            Name = ((TitlePropertyValue)page.Properties["Name"]).Title[0].PlainText
        }.UpdateSlug(wordsService).Synchronize();
    }
    
    public static Publication MapToPublication(this Page page,
        ICollection<Author> authors, ICollection<Publisher> publishers, IWordsService wordsService)
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
        }.UpdateSlug(wordsService).Synchronize();
    }
    
    public static Author MapToAuthor(this Page page, IWordsService wordsService)
    {
        return new Author
        {
            Id = (int)((UniqueIdPropertyValue)page.Properties["ID"]).UniqueId.Number!.Value,
            NotionId = Guid.Parse(page.Id),
            Name = ((TitlePropertyValue)page.Properties["Name"]).Title[0].PlainText,
            ProfileLink = ((UrlPropertyValue)page.Properties["Profile link"]).Url
        }.UpdateSlug(wordsService).Synchronize();
    }
}