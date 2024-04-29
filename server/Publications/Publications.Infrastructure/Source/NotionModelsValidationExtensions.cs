using Notion.Client;

namespace Publications.Infrastructure.Source;

public static class NotionModelsValidationExtensions
{
    public static bool IsValidPublication(this Page publicationPage)
    {
        if (publicationPage.Properties["ID"] is not UniqueIdPropertyValue id ||
            publicationPage.Properties["Name"] is not TitlePropertyValue title ||
            publicationPage.Properties["Type"] is not SelectPropertyValue type ||
            publicationPage.Properties["Year"] is not NumberPropertyValue year ||
            publicationPage.Properties["Abstract"] is not RichTextPropertyValue abstractValue ||
            publicationPage.Properties["Visible"] is not CheckboxPropertyValue visible)
            return false;
    
        return id.UniqueId.Number.HasValue &&
               title.Title.Count > 0 &&
               title.Title[0].PlainText.Length > 0 &&
               type.Select?.Name.Length > 0 &&
               year.Number.HasValue &&
               abstractValue.RichText.Count > 0 &&
               abstractValue.RichText[0].PlainText.Length > 0 &&
               visible.Checkbox;
    }
    
    public static bool IsValidAuthor(this Page authorPage)
    {
        if (authorPage.Properties["ID"] is not UniqueIdPropertyValue id ||
            authorPage.Properties["Name"] is not TitlePropertyValue name)
            return false;
    
        return id.UniqueId.Number.HasValue && 
               name.Title.Count > 0 &&
               name.Title[0].PlainText.Length > 0;
    }
    
    public static bool IsValidPublisher(this Page publisherPage)
    {
        if (publisherPage.Properties["ID"] is not UniqueIdPropertyValue id ||
            publisherPage.Properties["Name"] is not TitlePropertyValue name)
            return false;
    
        return id.UniqueId.Number.HasValue && 
               name.Title.Count > 0 &&
               name.Title[0].PlainText.Length > 0;
    }
}