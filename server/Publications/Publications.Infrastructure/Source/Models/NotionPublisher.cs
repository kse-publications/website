using Notion.Client;
using Publications.Domain.Publishers;

namespace Publications.Infrastructure.Source.Models;

internal class NotionPublisher : Publisher
{
    internal string NotionId { get; set; } = string.Empty;
    
    internal static NotionPublisher? MapFromPage(Page page)
    {
        if (!IsValidPage(page))
            return null;
        
        NotionPublisher publisher = new()
        {
            NotionId = page.Id,
            Id = (int)((UniqueIdPropertyValue)page.Properties["ID"]).UniqueId.Number!.Value,
            Name = ((TitlePropertyValue)page.Properties["Name"]).Title[0].PlainText
        };
        
        publisher.Synchronize();
        
        return publisher;
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
            SynchronizedAt = SynchronizedAt
        };
    }
}