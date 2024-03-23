using Redis.OM.Modeling;

namespace Publications.API.Models;

/// <summary>
/// Represents a publisher of a <see cref="Publication"/>. 
/// </summary>
public class Publisher
{
    [Indexed]
    public Guid Id { get; set; }
    
    [Searchable]
    public string Name { get; set; } = null!;
}