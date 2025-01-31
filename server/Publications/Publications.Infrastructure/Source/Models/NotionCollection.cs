using Notion.Client;
using Publications.Domain.Collections;

namespace Publications.Infrastructure.Source.Models;

internal class NotionCollection : Collection
{
    private readonly string _notionId;
    private readonly List<ObjectId> _publicationsRelation;
    private readonly List<int> _publicationsIds = [];
    private List<ObjectId> _ignoredPublicationsRelation = [];
    private readonly List<int> _ignoredPublicationsIds = [];
    
    internal string GetNotionId() => _notionId;

    private NotionCollection(
        string notionId,
        int id,
        string name,
        DateTime lastModifiedAt,
        List<ObjectId> publicationsRelation) 
        : base(id, name, lastModifiedAt)
    {
        _notionId = notionId;
        _publicationsRelation = publicationsRelation;
    }
    
    internal static NotionCollection? MapFromPage(Page page)
    {
        if (!(page.TryGetId(out int id) &&
              page.TryGetName(out string name) &&
              page.TryGetCheckBoxProperty("Visible", out bool visible) && visible &&
              page.TryGetRelationProperty("Publications", out var publications)))
            return null;

        return new NotionCollection(
            notionId: page.Id,
            id: id,
            name: name,
            lastModifiedAt: page.LastEditedTime,
            publicationsRelation: publications)
        {
            Icon = page.GetRichTextPropertyOrDefault("Icon"),
            Description = page.GetRichTextPropertyOrDefault("Description"),
            Keywords = page.GetSeparatedRichTextProperty("Auto Keywords", ','),
            _ignoredPublicationsRelation = page.GetRelationPropertyOrDefault("Ignored")
        };
    }
    
    internal static NotionCollection[] JoinPublications(
        IEnumerable<NotionCollection> collections,
        IEnumerable<NotionPublication> publications)
    {
        NotionCollection[] collectionsArray = collections.ToArray();
        NotionPublication[] publicationsArray = publications.ToArray();
        
        Dictionary<string, NotionCollection> collectionsDictionary = collectionsArray
            .ToDictionary(c => c.GetNotionId());
        
        Dictionary<string, int> publicationsDictionary = publicationsArray
            .ToDictionary(p => p.GetNotionId(), p => p.Id);

        foreach (var publication in publicationsArray)
        {
            foreach (ObjectId collectionRelation in publication.GetCollectionsRelation())
            {
                if (!collectionsDictionary.TryGetValue(collectionRelation.Id, out NotionCollection? collection))
                    continue;

                collection._publicationsIds.Add(publication.Id);
            }
        }

        foreach (var collection in collectionsArray)
        {
            foreach (ObjectId ignoredRelation in collection._ignoredPublicationsRelation)
            {
                if (!publicationsDictionary.TryGetValue(ignoredRelation.Id, out int publicationId))
                    continue;
                
                collection._ignoredPublicationsIds.Add(publicationId);
            }
        }

        return collectionsArray;
    }
    
    internal Collection ToCollection()
    {
        SetPublicationIds(_publicationsIds.ToArray());
        IgnoredPublicationIds = _ignoredPublicationsIds.ToArray();
        return this;
    }
}