using Notion.Client;
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
            Id = (int)((UniqueIdPropertyValue)page.Properties["ID"]).UniqueId.Number!.Value,
            Icon = ((RichTextPropertyValue)page.Properties["Icon"]).RichText
                .Select(r => r.PlainText).FirstOrDefault()!,
            Name = ((TitlePropertyValue)page.Properties["Name"]).Title[0].PlainText,
            Description = ((RichTextPropertyValue)page.Properties["Description"]).RichText
                .Select(r => r.PlainText).FirstOrDefault()!,
            PublicationsRelation = ((RelationPropertyValue)page.Properties["Publications"]).Relation,
            PublicationsCount = (int)((FormulaPropertyValue)page.Properties["Publications Count"]).Formula.Number!.Value
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