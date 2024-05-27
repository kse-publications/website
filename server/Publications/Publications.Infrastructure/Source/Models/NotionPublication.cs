﻿using Notion.Client;
using Publications.Domain.Publications;
using Publications.Domain.Shared.Slugs;

namespace Publications.Infrastructure.Source.Models;

internal class NotionPublication : Publication
{
    internal string NotionId { get; private init; } = string.Empty;
    private readonly ICollection<Collection> _updatableCollections = []; 
    
    internal static NotionPublication? MapFromPage(Page page, IWordsService wordsService)
    {
        if (!IsValidPage(page))
            return null;
        
        NotionPublication publication = new()
        {
            NotionId = page.Id,
            Id = (int)((UniqueIdPropertyValue)page.Properties["ID"]).UniqueId.Number!.Value,
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
    
    private static bool IsValidPage(Page publicationPage)
    {
        return publicationPage.Properties["ID"].IsValidId() &&
            publicationPage.Properties["Name"].IsValidName() &&
            IsValidType(publicationPage.Properties["Type"]) &&
            IsValidYear(publicationPage.Properties["Year"]) &&
            IsValidAbstract(publicationPage.Properties["Abstract"]) &&
            publicationPage.Properties["Visible"].IsVisible();
    }
    
    private static bool IsValidType(PropertyValue typeProperty) =>
        typeProperty is SelectPropertyValue type &&
        type.Select?.Name.Length > 0;
    
    private static bool IsValidYear(PropertyValue yearProperty) =>
        yearProperty is NumberPropertyValue year &&
        year.Number.HasValue;
    
    private static bool IsValidAbstract(PropertyValue abstractProperty) =>
        abstractProperty is RichTextPropertyValue abstractValue &&
        abstractValue.RichText.Count > 0 &&
        abstractValue.RichText[0].PlainText.Length > 0;
    
    internal NotionPublication JoinAuthors(Page page, ICollection<NotionAuthor> authors)
    {
        Authors = ((RelationPropertyValue)page.Properties["Authors"]).Relation
            .Select(r => authors.FirstOrDefault(a => a.NotionId == r.Id)?.ToAuthor())
            .Where(a => a is not null)
            .ToArray()!;
        
        return this;
    }
    
    internal NotionPublication JoinPublisher(
        Page page, ICollection<NotionPublisher> publishers)
    {
        var publisherRelation = ((RelationPropertyValue)page.Properties["Publisher"])?.Relation;
        if (publisherRelation?.FirstOrDefault()?.Id is not null)
        {
            var publisherId = publisherRelation[0].Id;
            Publisher = publishers
                .FirstOrDefault(p => p.NotionId == publisherId)?.ToPublisher();
        } else
        {
            Publisher = null;
        }
        
        return this;
    }
    
    internal static IEnumerable<NotionPublication> JoinCollections(
        IEnumerable<NotionPublication> publications, 
        IEnumerable<NotionCollection> collections)
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
                    publication._updatableCollections.Add(currentCollectionCopy);
                }
            }
        }

        return publicationsDictionary.Values;
    }

    internal Publication ToPublication()
    {
        return new Publication
        {
            Id = Id,
            Title = Title,
            Type = Type,
            Language = Language,
            Year = Year,
            Link = Link,
            Keywords = Keywords,
            Abstract = Abstract,
            Authors = Authors,
            Publisher = Publisher,
            Views = Views,
            Filters = Filters,
            Collections = _updatableCollections.ToArray(),
            Slug = Slug,
            SynchronizedAt = SynchronizedAt
        };
    }
}