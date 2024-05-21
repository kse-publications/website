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
    
        return id.IsValidId() && 
               title.IsValidName() &&
               type.Select?.Name.Length > 0 &&
               year.Number.HasValue &&
               abstractValue.RichText.Count > 0 &&
               abstractValue.RichText[0].PlainText.Length > 0 &&
               visible.IsVisible();
    }
    
    public static bool IsValidCollection(this Page collectionPage)
    {
        if (collectionPage.Properties["ID"] is not UniqueIdPropertyValue id ||
            collectionPage.Properties["Name"] is not TitlePropertyValue name ||
            collectionPage.Properties["Visible"] is not CheckboxPropertyValue visible) 
            return false;
    
        return id.IsValidId() && name.IsValidName() && visible.IsVisible();
    }
    
    public static bool IsValidAuthor(this Page authorPage)
    {
        if (authorPage.Properties["ID"] is not UniqueIdPropertyValue id ||
            authorPage.Properties["Name"] is not TitlePropertyValue name)
            return false;
    
        return id.IsValidId() && name.IsValidName();
    }
    
    public static bool IsValidPublisher(this Page publisherPage)
    {
        if (publisherPage.Properties["ID"] is not UniqueIdPropertyValue id ||
            publisherPage.Properties["Name"] is not TitlePropertyValue name)
            return false;
    
        return id.IsValidId() && name.IsValidName();
    }
    
    private static bool IsValidId(this PropertyValue propertyValue) =>
        propertyValue is UniqueIdPropertyValue id && id.UniqueId.Number.HasValue;   
    
    private static bool IsValidName(this PropertyValue propertyValue) =>
        propertyValue is TitlePropertyValue name &&
        name.Title.Count > 0 && 
        name.Title[0].PlainText.Length > 0;
    
    private static bool IsVisible(this PropertyValue propertyValue) =>
        propertyValue is CheckboxPropertyValue { Checkbox: true };
}