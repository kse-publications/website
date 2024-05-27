using Notion.Client;

namespace Publications.Infrastructure.Source;

internal static class NotionPropertyValueExtensions
{
    internal static int GetId(this PropertyValue idProperty) =>
        (int)((UniqueIdPropertyValue)idProperty).UniqueId.Number!.Value;
    
    internal static string GetName(this PropertyValue nameProperty) =>
        ((TitlePropertyValue)nameProperty).Title[0].PlainText;

    internal static string GetRichTextProperty(this Page page, string propertyName) =>
        page.Properties.TryGetValue(propertyName, out PropertyValue? iconProperty)
            ? ((RichTextPropertyValue)iconProperty).RichText
            .Select(r => r.PlainText).FirstOrDefault()!
            : string.Empty;

    internal static string[] GetSeparatedRichTextProperty(this Page page,
        string propertyName, char separator)
    {
        return page.Properties.TryGetValue(propertyName, out PropertyValue? richTextProperty)
            ? ((RichTextPropertyValue)richTextProperty).RichText
            .SelectMany(r 
                => r.PlainText.Split(separator, 
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .ToArray() 
            : Array.Empty<string>();
    }
    
    internal static string GetSelectProperty(this Page page, string propertyName) =>
        page.Properties.TryGetValue(propertyName, out PropertyValue? property)
            ? ((SelectPropertyValue)property).Select?.Name ?? string.Empty
            : string.Empty;
    
    internal static double GetNumberProperty(this Page page, string propertyName) =>
        page.Properties.TryGetValue(propertyName, out PropertyValue? property)
            ? ((NumberPropertyValue)property).Number!.Value
            : 0;

    internal static PropertyValue? GetNullableProperty(this Page page,
        string propertyName)
    {
        page.Properties.TryGetValue(propertyName, out PropertyValue? property);
        return property;
    }
    
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