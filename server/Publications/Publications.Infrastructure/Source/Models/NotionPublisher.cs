using Notion.Client;
using Publications.Domain.Publications;

namespace Publications.Infrastructure.Source.Models;

internal class NotionPublisher : Publisher
{
    internal string NotionId { get; set; } = string.Empty;
    
    internal static NotionPublisher? MapFromPage(Page page)
    {
        if (!IsValidPage(page))
            return null;
        
        return new NotionPublisher
        {
            NotionId = page.Id,
            Id = page.Properties["ID"].GetId(),
            Name = page.Properties["Name"].GetName()
        };
    }
    
    private static bool IsValidPage(Page publisherPage)
    {
        return publisherPage.Properties["ID"].IsValidId() && 
               publisherPage.Properties["Name"].IsValidName();
    }

    public Publisher ToPublisher()
    {
        return new Publisher
        {
            Id = Id,
            Name = Name,
        };
    }
}