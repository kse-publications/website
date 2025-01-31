using Notion.Client;
using Publications.Domain.Publications;

namespace Publications.Infrastructure.Source.Models;

internal class NotionPublication : Publication
{
    private readonly string _notionId;
    private readonly List<ObjectId> _collectionsRelation;

    private NotionPublication(
        string notionId,
        int id,
        string title,
        string type,
        int year,
        string link,
        string abstractText,
        DateTime lastModifiedAt,
        List<ObjectId> collectionsRelation) 
        : base(id, title, type, year, link, abstractText, lastModifiedAt)
    {
        _notionId = notionId;
        _collectionsRelation = collectionsRelation;
    }
    
    internal static NotionPublication? MapFromPage(Page page)
    {
        if (!(page.TryGetId(out int id) && 
              page.TryGetName(out string name) &&
              page.TryGetSelectProperty("Type", out string type) &&
              page.TryGetNumberProperty("Year", out double year) &&
              page.TryGetUrlProperty("Link", out string url) &&
              page.TryGetRichTextProperty("Abstract", out string abstractText) &&
              page.TryGetCheckBoxProperty("Visible", out bool visible) && visible))
            return null;

        var collections = page.GetRelationPropertyOrDefault("Collections");
        return new NotionPublication(
            notionId: page.Id,
            id: id,
            title: name,
            type: type,
            year: (int)year,
            link: url,
            abstractText: abstractText,
            lastModifiedAt: page.LastEditedTime,
            collectionsRelation: collections)
        {
            Language = page.GetSelectPropertyOrDefault("Language"),
            SearchableKeywords = page.GetSeparatedRichTextProperty("Keywords", ',')
                .Select(k => new Keyword { Value = k })
                .ToArray()
        };
    }
    
    internal string GetNotionId() => _notionId;
    internal List<ObjectId> GetCollectionsRelation() => _collectionsRelation;
    
    internal NotionPublication JoinAuthors(Page page, ICollection<NotionAuthor> authors)
    {
        if (!page.TryGetRelationProperty("Authors", out var authorsRelation))
            return this;
        
        Authors = authorsRelation
            .Select(r => authors.FirstOrDefault(a => a.GetNotionId() == r.Id)?.ToAuthor())
            .Where(a => a is not null)
            .ToArray()!;
        
        return this;
    }
    
    internal NotionPublication JoinPublisher(
        Page page, ICollection<NotionPublisher> publishers)
    {
        if (!page.TryGetRelationProperty("Publisher", out var publisherRelation))
            return this;
        
        if (publisherRelation.FirstOrDefault()?.Id is null)
        {
            Publisher = null;
            return this;
        }

        var publisherId = publisherRelation[0].Id;
        Publisher = publishers
            .FirstOrDefault(p => p.GetNotionId() == publisherId);

        return this;
    }

    internal Publication ToPublication()
    {
        return this;
    }
}