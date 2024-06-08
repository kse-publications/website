using Notion.Client;
using Publications.Domain.Publications;

namespace Publications.Infrastructure.Source.Models;

internal class NotionPublisher : Publisher
{
    private readonly string _notionId;
    internal string GetNotionId() => _notionId;
    
    private NotionPublisher(
        string notionId,
        int id,
        string name) 
        : base(id, name)
    {
        _notionId = notionId;
    }
    
    internal static NotionPublisher? MapFromPage(Page page)
    {
        if (!(page.TryGetId(out int id) && 
              page.TryGetName(out string name)))
            return null;
        
        return new NotionPublisher(
            notionId: page.Id,
            id: id,
            name: name);
    }
}