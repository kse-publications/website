namespace Publications.API.Abstractions;

public interface IPublication
{ 
    Guid Id { get; }
    string Title { get; }
    string Link { get; }
    string[] Keywords { get; }
}