using Redis.OM.Searching.Query;

namespace Publications.API.Repositories.Shared;

public static class RedisQueryExtensions
{
    public static RedisQuery Limit(this RedisQuery query, int pageSize, int page)
    {
        query.Limit = new SearchLimit
        {
            Number = pageSize,
            Offset = (page - 1) * pageSize
        };
        return query;
    }

    public static SearchRedisQuery Where(this RedisQuery query, string subquery)
    {
        query.QueryText = subquery;
        return new SearchRedisQuery(query);
    }

    public static string Search(this string propertyName, string searchTerm)
    {
        return $"(@{propertyName}:{searchTerm})";
    }

    public static string Prefix(this string propertyName, string searchTerm)
    {
        return $"(@{propertyName}:{searchTerm}*)";
    }
    
    public static string EqualTo(this string propertyName, string searchTerm)
    {
        return $"(@{propertyName}:{{{searchTerm}}})";
    }
}

