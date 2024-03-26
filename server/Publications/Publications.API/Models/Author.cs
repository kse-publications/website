
namespace Publications.API.Models;

/// <summary>
/// Represents an author of a <see cref="Publication"/>.
/// </summary>
public class Author
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string ProfileLink { get; set; } = string.Empty;
}