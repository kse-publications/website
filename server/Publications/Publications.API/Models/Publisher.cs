using Redis.OM.Modeling;

namespace Publications.API.Models;

public class Publisher
{
    [Indexed]
    public Guid Id { get; set; }
    
    [Searchable]
    public string Name { get; set; } = null!;
}