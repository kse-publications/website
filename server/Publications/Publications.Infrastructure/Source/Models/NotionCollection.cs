using Notion.Client;
using Publications.Domain.Collections;
using Publications.Domain.Publications;
using Publications.Domain.Shared.Slugs;

namespace Publications.Infrastructure.Source.Models;

internal class NotionCollection : Collection
{
    internal List<ObjectId> PublicationsRelation { get; private init; } = [];
    
    internal static NotionCollection? MapFromPage(Page page, IWordsService wordsService)
    {
        if (!IsValidPage(page))
            return null;
        
        NotionCollection collection = new() 
        {
            Id = page.Properties["ID"].GetId(),
            Icon = page.GetRichTextProperty("Icon"),
            Name = page.Properties["Name"].GetName(),
            Description = page.GetRichTextProperty("Description"),
            PublicationsRelation = ((RelationPropertyValue)page.Properties["Publications"]).Relation,
            PublicationsCount = (int)((FormulaPropertyValue)page.Properties["Publications Count"])
                .Formula.Number!.Value,
            LastModifiedAt = page.LastEditedTime
        };
        
        collection
            .UpdateSlug(wordsService)
            .Synchronize();
        
        return collection;
    }
    
    private static bool IsValidPage(Page collectionPage)
    {
        return collectionPage.Properties["ID"].IsValidId() &&
               collectionPage.Properties["Name"].IsValidName() &&
               collectionPage.Properties["Visible"].IsVisible();
    }
    
    internal Collection ToCollection()
    {
        return new Collection
        {
            Id = Id,
            Name = Name,
            Icon = Icon,
            Slug = Slug,
            Description = Description,
            PublicationsCount = PublicationsCount,
            SynchronizedAt = SynchronizedAt
        };
    }
}