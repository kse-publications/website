using Notion.Client;
using Publications.Domain.Collections;

namespace Publications.Infrastructure.Source.Models;

internal class NotionCollection : Collection
{
    private readonly List<ObjectId> _publicationsRelation;
    private readonly List<int> _publicationsIds = [];

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
              page.TryGetRelationProperty("Publications", out var publications)))
            return null;
        
        return new NotionCollection(
            id: id,
            name: name,
            lastModifiedAt: page.LastEditedTime,
            publicationsRelation: publications)
        {
            Icon = page.GetRichTextPropertyOrDefault("Icon"),
            Description = page.GetRichTextPropertyOrDefault("Description")
        };
    }
    
    internal List<ObjectId> GetPublicationsRelation() => _publicationsRelation;
    
    internal void AddPublicationId(int publicationId) => _publicationsIds.Add(publicationId);
    
    internal Collection ToCollection()
    {
        SetPublicationIds(_publicationsIds.ToArray());
        return this;
    }
}