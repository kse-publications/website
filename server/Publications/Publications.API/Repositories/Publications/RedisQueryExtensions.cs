using Publications.API.Models;
using Redis.OM.Searching.Query;

namespace Publications.API.Repositories.Publications;

public static class RedisQueryExtensions
{
    public static RedisQuery ApplyTypeFiltering(this RedisQuery query, string type)
    {
        if (type == string.Empty)
            return query;

        query.And(nameof(Publication.Type).EqualTo(type));
        return query;
    }

    public static RedisQuery Where(this RedisQuery query, string subquery)
    {
        query.QueryText = subquery;
        return query;
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

    public static RedisQuery Or(this RedisQuery query, string subquery)
    {
        query.QueryText = $"({query.QueryText} | {subquery})";
        return query;
    }

    public static RedisQuery And(this RedisQuery query, string subquery)
    {
        query.QueryText = $"{query.QueryText} {subquery}";
        return query;
    }
    
    public static RedisQuery Limit(this RedisQuery query, int pageSize, int page)
    {
        query.Limit = new SearchLimit
        {
            Number = pageSize,
            Offset = (page - 1) * pageSize
        };
        return query;
    }
}