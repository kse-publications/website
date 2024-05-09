using System.Linq.Expressions;
using System.Reflection;
using Publications.Application;
using Publications.Application.DTOs;
using Publications.Domain.Filters;
using Publications.Domain.Shared;
using Redis.OM;
using Redis.OM.Searching;

namespace Publications.Infrastructure.Shared.Queries;

internal static class RedisCollectionQueryExtensions
{
    internal static IRedisCollection<T> Filter<T>(this IRedisCollection<T> collection,
        FilterDTO filterDto) where T : Entity<T>
    {
        if (filterDto.GetParsedFilters().Length == 0)
            return collection;

        ParameterExpression entityParameter = Expression.Parameter(typeof(T), "e");
        ParameterExpression filterParameter = Expression.Parameter(typeof(Filter), "f");
        
        MethodInfo anyMethod = typeof(Enumerable).GetMethods()
            .First(m => 
                m.Name == nameof(Enumerable.Any) && 
                m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(Filter));
        
        MemberExpression filtersProperty = Expression.Property(entityParameter, nameof(Entity<T>.Filters));
        MemberExpression filtersIdProperty = Expression.Property(filterParameter, nameof(Domain.Filters.Filter.Id));
        
        Expression? body = null;

        foreach (int[] filters in filterDto.GetParsedFilters())
        {
            if (filters.Length == 0)
                continue;

            Expression? groupExpression = null;
            foreach (int filter in filters)
            {
                var equalsExpression = Expression.Equal(filtersIdProperty, Expression.Constant(filter));
                var anyLambda = Expression.Lambda<Func<Filter, bool>>(equalsExpression, filterParameter);
                var anyExpression = Expression.Call(anyMethod, filtersProperty, anyLambda);

                groupExpression = groupExpression is null 
                    ? anyExpression 
                    : Expression.OrElse(groupExpression, anyExpression);
            }
            
            if (body is null)
            {
                body = groupExpression;
                continue;
            }
            if (groupExpression is null)
                continue;
            
            body = Expression.AndAlso(body, groupExpression);
        }
        
        if (body is null)
            return collection;
        
        var lambda = Expression.Lambda<Func<T, bool>>(body, entityParameter);
        return collection.Where(lambda);
    }

    internal static async Task<PaginatedCollection<T>> GetPaginatedCollection<T>(
        this IRedisCollection<T> collection, PaginationFilterDTO paginationDTO) 
        where T : Entity<T>
    {
        Task<IList<T>> paginatedItemsTask = collection
            .Skip(paginationDTO.PageSize * (paginationDTO.Page - 1))
            .Take(paginationDTO.PageSize)
            .ToListAsync();
        
        Task<int> totalCountTask = collection.CountAsync();
        
        await Task.WhenAll(paginatedItemsTask, totalCountTask);

        IReadOnlyCollection<T> items = (await paginatedItemsTask).AsReadOnly();
        return new PaginatedCollection<T>(
            Items: items,
            ResultCount: items.Count,
            TotalCount: await totalCountTask);
    }
}