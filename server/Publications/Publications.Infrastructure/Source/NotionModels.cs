using Notion.Client;
using Publications.Domain.Authors;
using Publications.Domain.Publications;
using Publications.Domain.Publishers;

namespace Publications.Infrastructure.Source;


internal class NotionCollection : Collection
{
    public List<ObjectId> PublicationsRelation { get; set; } = [];
    
    public Collection ToCollection()
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

internal class NotionPublication : Publication
{
    public string NotionId { get; set; } = string.Empty;
    public ICollection<Collection> UpdatableCollections { get; set; } = [];

    public Publication ToPublication()
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
            Collections = UpdatableCollections.ToArray(),
            Slug = Slug,
            SynchronizedAt = SynchronizedAt
        };
    }
}

internal class NotionPublisher : Publisher
{
    public string NotionId { get; set; } = string.Empty;

    public Publisher ToPublisher()
    {
        return new Publisher
        {
            Id = Id,
            Name = Name,
            Slug = Slug,
            SynchronizedAt = SynchronizedAt
        };
    }
}

internal class NotionAuthor : Author
{
    public string NotionId { get; set; } = string.Empty;
    
    public Author ToAuthor()
    {
        return new Author
        {
            Id = Id,
            Name = Name,
            Slug = Slug,
            SynchronizedAt = SynchronizedAt
        };
    }
}