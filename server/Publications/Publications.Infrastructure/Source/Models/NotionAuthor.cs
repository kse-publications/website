using Notion.Client;
using Publications.Domain.Publications;

namespace Publications.Infrastructure.Source.Models;

internal class NotionAuthor : Author
{
    private readonly string _notionId;
    internal string GetNotionId() => _notionId;
    
    private NotionAuthor(
        string notionId,
        int id,
        string name,
        DateTime lastModifiedAt) 
        : base(id, name)
    {
        _notionId = notionId;
        LastModifiedAt = lastModifiedAt;
    }
    
    internal static NotionAuthor? MapFromPage(Page page)
    {
        if (!(page.TryGetId(out int id) && 
              page.TryGetName(out string name))) 
            return null;
        
        return new NotionAuthor(
            notionId: page.Id,
            id: id,
            name: name,
            lastModifiedAt: page.LastEditedTime);
    }
    
    internal Author ToAuthor()
    {
        return this;
    }
}