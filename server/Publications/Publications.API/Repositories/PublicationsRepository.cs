using System.Text.RegularExpressions;
using Publications.API.DTOs;
using Publications.API.Models;
using Publications.API.Repositories.Abstractions;
using Redis.OM;
using Redis.OM.Searching;

namespace Publications.API.Repositories;

public class PublicationsRepository: IPublicationsRepository
{
    private readonly RedisConnectionProvider _redisConnectionProvider;
    private readonly IRedisCollection<Publication> _publications;

    public PublicationsRepository(RedisConnectionProvider redisConnectionProvider)
    {
        _redisConnectionProvider = redisConnectionProvider;
        _publications = redisConnectionProvider.RedisCollection<Publication>();
    }

    public async Task<PaginatedCollection<Publication>> GetAllAsync(
        PaginationDTO paginationDto, CancellationToken cancellationToken = default)
    {
        IRedisCollection<Publication> sortedPublications = ApplySorting(
            _publications, paginationDto);
        
        IReadOnlyCollection<Publication> paginatedPublications = (await ApplyPagination(
            sortedPublications, paginationDto)
            .ToListAsync())
            .AsReadOnly();
        
        return new PaginatedCollection<Publication>(
            Items: paginatedPublications,
            ResultCount: paginatedPublications.Count,
            TotalCount: await _publications.CountAsync());
    }

    public async Task<PaginatedCollection<Publication>> GetByFullTextSearchAsync(
        PaginationSearchDTO paginationSearchDto,
        CancellationToken cancellationToken = default)
    {
        const int minSearchTermLength = 2;
        string searchTerm = SanitizeSearchTerm(paginationSearchDto.SearchTerm);
        
        if (searchTerm.Length < minSearchTermLength)
        {
            return new PaginatedCollection<Publication>(
                Items: new List<Publication>(),
                ResultCount: 0,
                TotalCount: await _publications.CountAsync());
        }
        
        IRedisCollection<Publication> matchedPublications = _publications
            .Where(p => 
                p.Title.Contains(searchTerm) || 
                p.Abstract.Contains(searchTerm) || 
                p.Keywords.Contains(searchTerm) ||
                p.Publisher.Name.Contains(searchTerm));
        
        
        IReadOnlyCollection<Publication> paginatedPublications = (await ApplyPagination(
            matchedPublications, paginationSearchDto)
            .ToListAsync())
            .AsReadOnly();
        
        return new PaginatedCollection<Publication>(
            Items: paginatedPublications,
            ResultCount: paginatedPublications.Count,
            TotalCount: await _publications.CountAsync());
    }

    public async Task<PaginatedCollection<Publication>> GetByAutoCompleteAsync(
        PaginationSearchDTO paginationSearchDto,
        CancellationToken cancellationToken = default)
    {
        const int minFuzzyMatchLength = 4;
        const int fuzzyMatchDistance = 1;
        
        string searchTerm = SanitizeSearchTerm(paginationSearchDto.SearchTerm);
        
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return new PaginatedCollection<Publication>(
                Items: new List<Publication>(),
                ResultCount: 0,
                TotalCount: await _publications.CountAsync());
        }
        
        IRedisCollection<Publication> matchedPublications;
        
        if (paginationSearchDto.SearchTerm.Length < minFuzzyMatchLength || 
            !AllowingFuzzyMatch(searchTerm))
        {
            matchedPublications = _publications.Where(p => 
                p.Title.StartsWith(searchTerm));
        }
        else
        {
            matchedPublications = _publications
                .Where(p => 
                    p.Title.StartsWith(searchTerm) ||
                    p.Title.FuzzyMatch(searchTerm, fuzzyMatchDistance));
        }
        
        IReadOnlyCollection<Publication> paginatedPublications = (await ApplyPagination(
                matchedPublications, paginationSearchDto)
            .ToListAsync())
            .AsReadOnly();
        
        return new PaginatedCollection<Publication>(
            Items: paginatedPublications,
            ResultCount: paginatedPublications.Count,
            TotalCount: await _publications.CountAsync());
    }

    public async Task<Publication?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await _publications.FindByIdAsync(id.ToString());
    }

    public async Task InsertOrUpdateAsync(
        IEnumerable<Publication> publications,
        CancellationToken cancellationToken = default)
    {
        await _publications.InsertAsync(publications);
    }
    
    private IRedisCollection<Publication> ApplyPagination(
        IRedisCollection<Publication> publications, PaginationDTO paginationDto)
    {
        return publications
            .Skip((paginationDto.Page - 1) * paginationDto.PageSize)
            .Take(paginationDto.PageSize);
    }

    private IRedisCollection<Publication> ApplySorting(
        IRedisCollection<Publication> publications, PaginationDTO paginationDto)
    {
        return (paginationDto.SortBy.ToLower(), paginationDto.Ascending) switch
        {
            ("type", true) => publications.OrderBy(p => p.Type),
            ("type", false) => publications.OrderByDescending(p => p.Year),
            ("year", true) => publications.OrderBy(p => p.Year),
            ("year", false) => publications.OrderByDescending(p => p.Year),
            ("lastmodified", true) => publications.OrderBy(p => p.LastModified),
            _ => publications.OrderByDescending(p => p.LastModified)
        };
    }
    
    private static string SanitizeSearchTerm(string searchTerm)
    {
        string sanitizedTerm = Regex.Replace(searchTerm, "[\\~#%&*{}/:<>|\"-\\\\[\\\\]]", "");
        Console.WriteLine(sanitizedTerm);
        sanitizedTerm = Regex.Replace(sanitizedTerm, @"\s+", " ").Trim();
        return sanitizedTerm.Substring(0, Math.Min(50, sanitizedTerm.Length));
    }
    
    private static bool AllowingFuzzyMatch(string searchTerm)
    {
        const string forbiddenChars = "0123456789!@#$%^&*()_+{}|:\"<>?`~-=\\\\\\[\\\\];',./";
        return searchTerm.Length >= 4 && !searchTerm.All(c => forbiddenChars.Contains(c));
    }
}