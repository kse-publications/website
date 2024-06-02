using Notion.Client;
using Publications.Domain.Publications;

namespace Publications.Infrastructure.Source.Models;

internal class NotionAuthor : Author
{
    internal string NotionId { get; private init; } = string.Empty;
    
    internal static NotionAuthor? MapFromPage(Page page)
    {
        if (!IsValidPage(page))
            return null;
        
        return new NotionAuthor
        {
            NotionId = page.Id,
            Id = page.Properties["ID"].GetId(),
            Name = page.Properties["Name"].GetName()
        };
    }
    
    private static bool IsValidPage(Page authorPage)
    {
        return authorPage.Properties["ID"].IsValidId() &&
               authorPage.Properties["Name"].IsValidName();
    }
    
    internal Author ToAuthor()
    {
        return new Author
        {
            Id = Id,
            Name = Name,
        };
    }
}