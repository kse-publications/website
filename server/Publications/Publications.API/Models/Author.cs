using Redis.OM.Modeling;

namespace Publications.API.Models;

/// <summary>
/// Represents an author of a <see cref="Publication"/>.
/// </summary>
public class Author
{
    [Indexed]
    public Guid Id { get; set; }
    
    [Indexed]
    public string Name { get; set; } = null!;
    
    public string ProfileLink { get; set; } = string.Empty;
}