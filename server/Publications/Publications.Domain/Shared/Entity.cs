using System.Text.Json.Serialization;
using Publications.Domain.Shared.Serialization;
using Redis.OM.Modeling;

namespace Publications.Domain.Shared;

public abstract class Entity<T> where T: Entity<T>
{
    [RedisIdField]
    [Indexed(Sortable = true)]
    public int Id { get; init; }
    
    [Indexed]
    [IgnoreInResponse]
    [JsonConverter(typeof(UnixTimestampJsonConverter))]
    public DateTime SynchronizedAt { get; set; }
    
    [Indexed]
    [IgnoreInResponse]
    [JsonConverter(typeof(UnixTimestampJsonConverter))]
    public DateTime LastModifiedAt { get; set; }

    public void Synchronize()
    {
        SynchronizedAt = DateTime.UtcNow;
    }
    
    public static string IndexName => $"{typeof(T).Name.ToLower()}-idx";
}