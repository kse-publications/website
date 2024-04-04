using Redis.OM.Modeling;

namespace Publications.API.Models;

public abstract class Entity<T> where T: Entity<T>
{
    
    [RedisIdField]
    [Indexed]
    [IgnoreInResponse]
    public int Id { get; set; }
    
    [IgnoreInResponse]
    public Guid NotionId { get; set; }
    
    public string Slug { get; set; } = string.Empty;
    
    public abstract T UpdateSlug();
}