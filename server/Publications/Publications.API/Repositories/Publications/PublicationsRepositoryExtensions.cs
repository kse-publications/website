using Publications.API.DTOs;
using Publications.API.Models;
using Redis.OM;
using Redis.OM.Searching;

namespace Publications.API.Repositories.Publications;

public static class PublicationsRepositoryExtensions
{
    public static async Task<PaginatedCollection<Publication>> ApplyPagination(
        this IRedisCollection<Publication> publications,
        int pageNumber,
        int pageSize,
        int totalMatches)
    {
       IReadOnlyCollection<Publication> paginatedPublications = (await publications
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync())
           .AsReadOnly();
       
       return new PaginatedCollection<Publication>(
           Items: paginatedPublications,
           ResultCount: paginatedPublications.Count,
           TotalCount: totalMatches);
    }
    
    public static IRedisCollection<Publication> ApplyFiltering(
        this IRedisCollection<Publication> publications, PaginationFilterDTO paginationFilterDTO)
    {
        return paginationFilterDTO.Filter == string.Empty
            ? publications
            : publications.Where(p => p.Type == paginationFilterDTO.Filter);
    }
    
    public static IRedisCollection<Publication> OnlyVisible(
        this IRedisCollection<Publication> publications)
    {
        return publications.Where(p => p.Visible == "true");
    }
    
    public static IRedisCollection<Publication> ApplySorting(
        this IRedisCollection<Publication> publications, string sortBy, bool isAscending)
    {
        return (sortBy.ToLower(), isAscending) switch
        {
            ("title", true) => publications.OrderBy(p => p.Title),
            ("title", false) => publications.OrderByDescending(p => p.Title),
            ("year", true) => publications.OrderBy(p => p.Year),
            ("year", false) => publications.OrderByDescending(p => p.Year),
            ("type", true) => publications.OrderBy(p => p.Type),
            ("type", false) => publications.OrderByDescending(p => p.Type),
            ("lastmodified", true) => publications.OrderBy(p => p.LastModified),
            _ => publications.OrderByDescending(p => p.LastModified)
        };
    }
}