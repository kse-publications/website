using Redis.OM.Modeling;

namespace Publications.API.Models;

public class Filter
{
    [Indexed]
    public int Id { get; set; }
    
    public string Value { get; set; } = null!;
}