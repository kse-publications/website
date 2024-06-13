using Notion.Client;

namespace Publications.Infrastructure.Source;

internal static class NotionPropertyValueExtensions
{
    internal static bool IsValidId(this PropertyValue idProperty) =>
        idProperty is UniqueIdPropertyValue id && 
        id.UniqueId.Number.HasValue;
    
    internal static bool IsValidName(this PropertyValue nameProperty) =>
        nameProperty is TitlePropertyValue name &&
        name.Title.Count > 0 && 
        name.Title[0].PlainText.Length > 0;
    
    internal static bool IsValidSelectProperty(this PropertyValue typeProperty) =>
        typeProperty is SelectPropertyValue type &&
        type.Select?.Name.Length > 0;
    
    internal static bool IsValidRichTextProperty(this PropertyValue abstractProperty) =>
        abstractProperty is RichTextPropertyValue abstractValue &&
        abstractValue.RichText.Count > 0 &&
        abstractValue.RichText[0].PlainText.Length > 0;
    
    internal static bool IsValidUrlProperty(this PropertyValue? urlProperty) =>
        urlProperty is UrlPropertyValue url &&
        !string.IsNullOrWhiteSpace(url.Url);
    
    internal static bool IsValidNumberProperty(this PropertyValue numberProperty) =>
        numberProperty is NumberPropertyValue number &&
        number.Number.HasValue; 
    
    internal static bool IsValidCheckBoxProperty(this PropertyValue checkBoxProperty) =>
        checkBoxProperty is CheckboxPropertyValue checkBox &&
        checkBox.Checkbox;
}