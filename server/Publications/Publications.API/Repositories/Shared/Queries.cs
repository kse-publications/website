using Redis.OM.Searching.Query;

namespace Publications.API.Repositories.Shared;

public static class Queries
{
    public static string Either(string expression, string subquery)
    {
        return $"({expression} | {subquery})";
    }
    
    public static string Both(string expression, string subquery)
    {
        return $"{expression} {subquery}";
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
    
    public static string EqualTo(this string propertyName, int numericValue)
    {
        return $"(@{propertyName}:[{numericValue},{numericValue}])";
    }
}