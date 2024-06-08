using Notion.Client;
using Publications.Domain.Collections;
using Publications.Domain.Publications;
using Publications.Domain.Shared.Slugs;

namespace Publications.Infrastructure.Source.Models;

internal class NotionPublication : Publication
{
    private readonly string _notionId;
    private string GetNotionId() => _notionId;
    
    private readonly ICollection<Collection> _updatableCollections = [];

    private NotionPublication(
        string notionId,
        int id,
        string title,
        string type,
        int year,
        string link,
        string abstractText,
        DateTime lastModifiedAt,
        IWordsService wordsService) 
        : base(id, title, type, year, link, abstractText, lastModifiedAt, wordsService)
    {
        _notionId = notionId;
    }
    
    internal static NotionPublication? MapFromPage(Page page, IWordsService wordsService)
    {
        if (!(page.TryGetId(out int id) && 
              page.TryGetName(out string name) &&
              page.TryGetSelectProperty("Type", out string type) &&
              page.TryGetNumberProperty("Year", out double year) &&
              page.TryGetUrlProperty("Link", out string url) &&
              page.TryGetRichTextProperty("Abstract", out string abstractText) &&
              page.TryGetCheckBoxProperty("Visible", out bool visible) && visible))
            return null;
        
        return new NotionPublication(
            notionId: page.Id,
            id: id,
            title: name,
            type: type,
            year: (int)year,
            link: url,
            abstractText: abstractText,
            lastModifiedAt: page.LastEditedTime,
            wordsService)
        {
            Language = page.GetSelectPropertyOrDefault("Language"),
            Keywords = page.GetSeparatedRichTextProperty("Keywords", separator: ','),
        };
    }
    
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
        
        if (!(publisherRelation.FirstOrDefault()?.Id is null))
        {
            Publisher = null;
            return this;
        }

        var publisherId = publisherRelation[0].Id;
        Publisher = publishers
            .FirstOrDefault(p => p.GetNotionId() == publisherId);

        return this;
    }
    
    internal static IEnumerable<NotionPublication> JoinCollections(
        IEnumerable<NotionPublication> publications, 
        IEnumerable<NotionCollection> collections)
    {
        Dictionary<string, NotionPublication> publicationsDictionary = publications
            .ToDictionary(p => p.GetNotionId());

        foreach (var collection in collections)
        {
            foreach (var relationId in collection.GetPublicationsRelation())
            {
                if (publicationsDictionary.TryGetValue(relationId.Id, out NotionPublication? publication))
                {
                    publication._updatableCollections.Add(collection);
                }
            }
        }

        return publicationsDictionary.Values;
    }

    internal Publication ToPublication()
    {
        Collections = _updatableCollections.ToArray();
        return this;
    }
}