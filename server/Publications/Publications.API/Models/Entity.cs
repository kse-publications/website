using Publications.API.Serialization;
using Redis.OM.Modeling;

namespace Publications.API.Models;

public abstract class Entity<T> where T: Entity<T>
{
    [RedisIdField]
    [Indexed]
    [IgnoreInResponse]
    public int Id { get; init; }
    
    [IgnoreInResponse]
    public Guid NotionId { get; init; }
    
    public string Slug { get; protected set; } = string.Empty;
    
    [Indexed(Sortable = true)]
    public int Views { get; private set; } 
    
    [Indexed(JsonPath = "$.Id")]
    public Filter[] Filters { get; set; } = Array.Empty<Filter>();
    
    public abstract T UpdateSlug();

    public T UpdateViews(int views = 1)
    {
        if (views < 0)
        {
            throw new ArgumentException("Views cannot be negative");
        }
        
        Views = views;
        return (T)this;
    }
    
    public static string IndexName => $"{typeof(T).Name.ToLower()}-idx";
}