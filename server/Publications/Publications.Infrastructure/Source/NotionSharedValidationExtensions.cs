using Notion.Client;

namespace Publications.Infrastructure.Source;

internal static class NotionSharedValidationExtensions
{
    internal static bool IsValidId(this PropertyValue idProperty) =>
        idProperty is UniqueIdPropertyValue id && 
        id.UniqueId.Number.HasValue;   
    
    internal static bool IsValidName(this PropertyValue nameProperty) =>
        nameProperty is TitlePropertyValue name &&
        name.Title.Count > 0 && 
        name.Title[0].PlainText.Length > 0;
    
    internal static bool IsVisible(this PropertyValue visibilityProperty) =>
        visibilityProperty is CheckboxPropertyValue checkBox &&
        checkBox.Checkbox;
}