using Notion.Client;
using Publications.Domain.Publications;
using Publications.Domain.Shared.Slugs;

namespace Publications.Infrastructure.Source;

public static class NotionModelsMapperExtensions
{
    internal static NotionPublication MapToPublication(this Page page, 
        IWordsService wordsService)
    {
        NotionPublication publication = new()
        {
            Id = (int)((UniqueIdPropertyValue)page.Properties["ID"]).UniqueId.Number!.Value,
            NotionId = page.Id,
            Title = ((TitlePropertyValue)page.Properties["Name"]).Title[0].PlainText,
            Type = ((SelectPropertyValue)page.Properties["Type"]).Select.Name,
            Language = ((SelectPropertyValue)page.Properties["Language"]).Select?.Name ?? string.Empty,
            Year = (int)((NumberPropertyValue)page.Properties["Year"]).Number!.Value,
            Link = ((UrlPropertyValue)page.Properties["Link"]).Url,

            Keywords = ((RichTextPropertyValue)page.Properties["Keywords"]).RichText
                .SelectMany(r => r.PlainText.Split(',',
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                .ToArray(),

            Abstract = ((RichTextPropertyValue)page.Properties["Abstract"])
                .RichText.Select(r => r.PlainText).FirstOrDefault()!,
        };
        
        publication
            .UpdateSlug(wordsService)
            .Synchronize();
        
        return publication;
    }
    
    internal static NotionPublication LinkAuthors(this NotionPublication publication,
        Page page, ICollection<NotionAuthor> authors)
    {
        publication.Authors = ((RelationPropertyValue)page.Properties["Authors"]).Relation
            .Select(r => authors.FirstOrDefault(a => a.NotionId == r.Id)?.ToAuthor())
            .Where(a => a is not null)
            .ToArray()!;
        
        return publication;
    }
    
    internal static NotionPublication LinkPublisher(this NotionPublication publication,
        Page page, ICollection<NotionPublisher> publishers)
    {
        var publisherRelation = ((RelationPropertyValue)page.Properties["Publisher"])?.Relation;
        if (publisherRelation?.FirstOrDefault()?.Id is not null)
        {
            var publisherId = publisherRelation[0].Id;
            publication.Publisher = publishers
                .FirstOrDefault(p => p.NotionId == publisherId)?.ToPublisher();
        } 
        else
        {
            publication.Publisher = null;
        }
        
        return publication;
    }
    
    internal static IEnumerable<Publication> LinkCollections(
        this IEnumerable<NotionPublication> publications, 
        ICollection<NotionCollection> collections)
    {
        Dictionary<string, NotionPublication> publicationsDictionary = publications
            .ToDictionary(p => p.NotionId);

        foreach (var collection in collections)
        {
            Collection currentCollectionCopy = collection.ToCollection();
            foreach (var relationId in collection.PublicationsRelation)
            {
                if (publicationsDictionary.TryGetValue(relationId.Id, out NotionPublication? publication))
                {
                    publication.UpdatableCollections.Add(currentCollectionCopy);
                }
            }
        }

        return publicationsDictionary.Values
            .Select(p => p.ToPublication());
    }
    
    internal static NotionCollection MapToCollection(this Page page, IWordsService wordsService)
    {
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
        
        collection.UpdateSlug(wordsService)
            .Synchronize();
        
        return collection;
    }
    
    internal static NotionAuthor MapToAuthor(this Page page, IWordsService wordsService)
    {
        NotionAuthor author = new()
        {
            Id = (int)((UniqueIdPropertyValue)page.Properties["ID"]).UniqueId.Number!.Value,
            NotionId = page.Id,
            Name = ((TitlePropertyValue)page.Properties["Name"]).Title[0].PlainText,
            ProfileLink = ((UrlPropertyValue)page.Properties["Profile link"]).Url
        };
        
        author.UpdateSlug(wordsService)
            .Synchronize();
        
        return author;
    }
    
    internal static NotionPublisher MapToPublisher(this Page page, IWordsService wordsService)
    {
        NotionPublisher publisher = new()
        {
            Id = (int)((UniqueIdPropertyValue)page.Properties["ID"]).UniqueId.Number!.Value,
            NotionId = page.Id,
            Name = ((TitlePropertyValue)page.Properties["Name"]).Title[0].PlainText
        };
        
        publisher.UpdateSlug(wordsService)
            .Synchronize();
        
        return publisher;
    }
}