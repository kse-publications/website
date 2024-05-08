using Publications.Application.DTOs;
using Publications.Domain.Publications;
using Publications.Domain.Shared;
using Redis.OM.Searching;
using Redis.OM.Searching.Query;

namespace Publications.Infrastructure.Shared;

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
                filterGroupQuery = Queries.Either(
                    filterGroupQuery, FilterQuery(filter));
            }

            query.QueryText = Queries.Both(query.QueryText, filterGroupQuery);
        }
        
        return query;
    }
    
    private static string FilterQuery(int filterId) =>
        $"{nameof(Entity<Publication>.Filters)}_{nameof(Domain.Filters.Filter.Id)}".EqualTo(filterId);
}

