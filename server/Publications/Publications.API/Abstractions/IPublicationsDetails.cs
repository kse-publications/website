namespace Publications.API.Abstractions;

public interface IPublicationsDetails
{
    Guid Id { get; }
    string Title { get; }
    string Type { get; }
    int Year { get; }
    string Link { get; }
    ICollection<IAuthor> Authors { get; }
    IPublisher Publishers { get; }
    string[] Keywords { get; }
    string Abstract { get; }
}