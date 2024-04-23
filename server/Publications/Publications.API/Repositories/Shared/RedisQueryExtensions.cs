using Publications.API.DTOs;
using Publications.API.Models;
using Redis.OM.Searching;
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
    
    public static RedisQuery Sort(this RedisQuery query,
        string propertyName, SortDirection direction)
    {
        query.SortBy = new RedisSortBy
        {
            Field = propertyName,
            Direction = direction
        };
        
        return query;
    }
    
    public static RedisQuery Filter(this RedisQuery query, FilterDTO filterDTO)
    {
        if (query.QueryText.Equals("*") && filterDTO.GetParsedFilters().Length != 0)
            query.QueryText = string.Empty;
        
        foreach (int[] filters in filterDTO.GetParsedFilters())
        {
            if (filters.Length == 0)
                continue;

            string filterGroupQuery = FilterQuery(filters.First());
            foreach (var filter in filters.Skip(1))
            {
                filterGroupQuery = Either(
                    filterGroupQuery, FilterQuery(filter));
            }

            query.QueryText = Both(query.QueryText, filterGroupQuery);
        }
        
        return query;
    }
    
    private static string FilterQuery(int filterId) =>
        $"{nameof(Entity<Publication>.Filters)}_{nameof(Models.Filter.Id)}".EqualTo(filterId);
    
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

