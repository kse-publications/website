using Redis.OM.Modeling;

namespace Publications.API.Models;

/// <summary>
/// Represents a publisher of a <see cref="Publication"/>. 
/// </summary>
public class Publisher
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;
}