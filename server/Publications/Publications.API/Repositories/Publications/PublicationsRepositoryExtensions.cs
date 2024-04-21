using Publications.API.DTOs;
using Publications.API.Models;
using Publications.API.Repositories.Shared;
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
        this IRedisCollection<Publication> publications,
        PublicationsPaginationFilterDTO paginationDTO)
    {
        return publications
            .FilterType(paginationDTO.Type)
            .FilterYear(paginationDTO.Year)
            .FilterLanguage(paginationDTO.Language);
    }
    
    private static IRedisCollection<Publication> FilterYear(
        this IRedisCollection<Publication> publications,
        int year)
    {
        if (year == default)
            return publications;
        
        return publications.Where(p => p.Year == year);
    }
    
    private static IRedisCollection<Publication> FilterType(
        this IRedisCollection<Publication> publications, string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return publications;
        
        return publications.Where(p => p.Type == type);
    }
    
    private static IRedisCollection<Publication> FilterLanguage(
        this IRedisCollection<Publication> publications, string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return publications;
        
        return publications.Where(p => p.Language == language);
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