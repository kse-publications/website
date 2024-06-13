using Notion.Client;

namespace Publications.Infrastructure.Source;

internal static class NotionPageExtensions
{
    internal static bool TryGetId(this Page page, out int id, string idPropertyName = "ID")
    {
        id = -1;
        if (!page.Properties.TryGetValue(idPropertyName, out PropertyValue? idProperty))
            return false;

        if (!idProperty.IsValidId())
            return false;

        id = (int)((UniqueIdPropertyValue)idProperty).UniqueId.Number!;
        return true;
    }
    
    internal static bool TryGetName(this Page page, out string name, string namePropertyName = "Name")
    {
        name = string.Empty;
        if (!page.Properties.TryGetValue(namePropertyName, out PropertyValue? nameProperty)) 
            return false;
        
        if (!nameProperty.IsValidName())
            return false;

        name = ((TitlePropertyValue)nameProperty).Title[0].PlainText;
        return true;
    }

    internal static bool TryGetRichTextProperty(this Page page, string propertyName, out string value)
    {
        value = string.Empty;
        if (!page.Properties.TryGetValue(propertyName, out PropertyValue? richTextProperty))
            return false;
        
        if (!richTextProperty.IsValidRichTextProperty())
            return false;

        value = ((RichTextPropertyValue)richTextProperty).RichText[0].PlainText;
        return true;
    }
    
    internal static string GetRichTextPropertyOrDefault(this Page page, string propertyName)
        => page.TryGetRichTextProperty(propertyName, out string value) ? value : string.Empty;

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

    internal static bool TryGetSelectProperty(this Page page, string propertyName, out string value)
    {
        value = string.Empty;
        if (!page.Properties.TryGetValue(propertyName, out PropertyValue? property))
            return false;

        if (!property.IsValidSelectProperty())
            return false;

        value = ((SelectPropertyValue)property).Select?.Name ?? string.Empty;
        return true;
    }
    
    internal static string GetSelectPropertyOrDefault(this Page page, string propertyName)
        => page.TryGetSelectProperty(propertyName, out string value) ? value : string.Empty;
    
    internal static bool TryGetUrlProperty(this Page page, string urlPropertyName, out string url)
    {
        url = string.Empty;
        if (!page.Properties.TryGetValue(urlPropertyName, out PropertyValue? urlProperty))
            return false;

        if (!urlProperty.IsValidUrlProperty())
            return false;

        url = ((UrlPropertyValue)urlProperty).Url;
        return true;
    }

    internal static bool TryGetNumberProperty(this Page page, string propertyName, out double number)
    {
        number = 0.0;
        if (!page.Properties.TryGetValue(propertyName, out PropertyValue? property))
            return false;
        
        if (!property.IsValidNumberProperty())
            return false;

        number = ((NumberPropertyValue)property).Number!.Value;
        return true;
    }
    
    internal static double GetNumberPropertyOrDefault(this Page page, string propertyName)
        => page.TryGetNumberProperty(propertyName, out double number) ? number : 0.0;
    
    internal static bool TryGetFormulaProperty(this Page page, string propertyName, out double formula)
    {
        formula = 0.0;
        if (!page.Properties.TryGetValue(propertyName, out PropertyValue? property))
            return false;

        if (property is not FormulaPropertyValue formulaProperty || 
            !formulaProperty.Formula.Number.HasValue)
            return false;

        formula = formulaProperty.Formula.Number!.Value;
        return true;
    }

    internal static double GetFormulaPropertyOrDefault(this Page page, string propertyName) 
        => page.TryGetFormulaProperty(propertyName, out double formula) ? formula : 0.0;
    
    internal static bool TryGetRelationProperty(this Page page,
        string propertyName, out List<ObjectId> relation)
    {
        relation = [];
        if (!page.Properties.TryGetValue(propertyName, out PropertyValue? property))
            return false;

        if (property is not RelationPropertyValue relationProperty ||
            relationProperty.Relation is null)
            return false;

        relation = relationProperty.Relation;
        return true;
    }

    internal static List<ObjectId> GetRelationPropertyOrDefault(this Page page, string propertyName) 
        => page.TryGetRelationProperty(propertyName, out List<ObjectId> relation) ? relation : [];

    internal static bool TryGetCheckBoxProperty(this Page page, string propertyName, out bool checkBoxValue)
    {
        checkBoxValue = false;
        if (!page.Properties.TryGetValue(propertyName, out PropertyValue? checkBoxProperty))
            return false;

        if (!checkBoxProperty.IsValidCheckBoxProperty())
            return false;
        
        checkBoxValue = ((CheckboxPropertyValue)checkBoxProperty).Checkbox;
        return true;
    }
    
    internal static PropertyValue? GetNullableProperty(this Page page, string propertyName)
    {
        page.Properties.TryGetValue(propertyName, out PropertyValue? property);
        return property;
    }
}