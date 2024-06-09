using Notion.Client;
using Publications.Domain.Collections;
using Publications.Domain.Shared.Slugs;

namespace Publications.Infrastructure.Source.Models;

internal class NotionCollection : Collection
{
    private readonly List<ObjectId> _publicationsRelation;
    internal List<ObjectId> GetPublicationsRelation() => _publicationsRelation;
    
    private readonly List<int> _publicationsIds = [];
    internal void AddPublicationId(int publicationId) 
        => _publicationsIds.Add(publicationId);

    private NotionCollection(
        int id,
        string name,
        DateTime lastModifiedAt,
        List<ObjectId> publicationsRelation,
        IWordsService wordsService) 
        : base(id, name, lastModifiedAt, wordsService)
    {
        _publicationsRelation = publicationsRelation;
    }
    
    internal static NotionCollection? MapFromPage(Page page, IWordsService wordsService)
    {
        if (!(page.TryGetId(out int id) &&
              page.TryGetName(out string name) &&
              page.TryGetRelationProperty("Publications", out var publications)))
            return null;
        
        return new NotionCollection(
            id: id,
            name: name,
            lastModifiedAt: page.LastEditedTime,
            publicationsRelation: publications,
            wordsService)
        {
            Icon = page.GetRichTextPropertyOrDefault("Icon"),
            Description = page.GetRichTextPropertyOrDefault("Description")
        };
    }
    
    internal Collection ToCollection()
    {
        SetPublicationIds(_publicationsIds.ToArray());
        return this;
    }
}