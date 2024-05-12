using System.Text.Json.Serialization;
using Publications.Domain.Filters;
using Publications.Domain.Shared.Attributes;
using Redis.OM.Modeling;

namespace Publications.Domain.Shared;

public abstract class Entity<T> where T: Entity<T>
{
    [RedisIdField]
    [Indexed(Sortable = true)]
    [IgnoreInResponse]
    public int Id { get; init; }

    [Indexed(Sortable = true)]
    public string Slug { get; set; } = string.Empty;
    
    [Indexed(Sortable = true)]
    public int Views { get; set; } 
    
    [Indexed(JsonPath = "$.Id")]
    [IgnoreInResponse]
    public Filter[] Filters { get; set; } = Array.Empty<Filter>();
    
    [IgnoreInResponse]
    public Guid NotionId { get; init; }
    
    [Indexed]
    [IgnoreInResponse]
    [JsonConverter(typeof(UnixTimestampJsonConverter))]
    public DateTime SynchronizedAt { get; set; }
    
    public abstract T UpdateSlug(IWordsService wordsService);

    public T UpdateViews(int views = 1)
    {
        if (views < 0)
        {
            throw new ArgumentException("Views cannot be negative");
        }
        
        Views = views;
        return (T)this;
    }
    
    public T Synchronize()
    {
        SynchronizedAt = DateTime.UtcNow;
        return (T)this;
    }
    
    public static string IndexName => $"{typeof(T).Name.ToLower()}-idx";
}