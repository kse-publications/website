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
        string name) 
        : base(id, name)
    {
        _notionId = notionId;
    }
    
    internal static NotionAuthor? MapFromPage(Page page)
    {
        if (!(page.TryGetId(out int id) && 
              page.TryGetName(out string name))) 
            return null;
        
        return new NotionAuthor(
            notionId: page.Id,
            id: id,
            name: name);
    }
    
    internal Author ToAuthor()
    {
        return this;
    }
}