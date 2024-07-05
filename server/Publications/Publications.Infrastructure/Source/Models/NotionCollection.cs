using Notion.Client;
using Publications.Domain.Collections;

namespace Publications.Infrastructure.Source.Models;

internal class NotionCollection : Collection
{
    private readonly List<ObjectId> _publicationsRelation;
    private readonly List<int> _publicationsIds = [];
    private List<ObjectId> _ignoredPublicationsRelation = [];
    private readonly List<int> _ignoredPublicationsIds = [];

    private NotionCollection(
        int id,
        string name,
        DateTime lastModifiedAt,
        List<ObjectId> publicationsRelation) 
        : base(id, name, lastModifiedAt)
    {
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
        Dictionary<string, int> publicationsDictionary = publications
            .ToDictionary(p => p.GetNotionId(), p => p.Id);
        
        NotionCollection[] collectionsArray = collections.ToArray();

        foreach (var collection in collectionsArray)
        {
            foreach (var relationId in collection._publicationsRelation)
            {
                if (!publicationsDictionary.TryGetValue(relationId.Id, out int publicationId)) 
                    continue;
                
                collection._publicationsIds.Add(publicationId);
            }

            foreach (var ignoredId in collection._ignoredPublicationsRelation)
            {
                if (!publicationsDictionary.TryGetValue(ignoredId.Id, out int publicationId)) 
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