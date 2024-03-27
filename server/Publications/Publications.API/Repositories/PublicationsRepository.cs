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
            TotalCount: await sortedPublications.CountAsync());
    }

    public async Task<PaginatedCollection<Publication>> GetBySearchAsync(
        PaginationSearchDTO paginationSearchDto,
        bool allowFuzzyMatch,
        CancellationToken cancellationToken = default)
    {
        const int fuzzyMatchDistance = 1;
        IRedisCollection<Publication> matchedPublications;

        if (!allowFuzzyMatch)
        {
            matchedPublications = _publications
                .Where(p =>
                    p.Title.Contains(paginationSearchDto.SearchTerm) ||
                    p.Title.StartsWith(paginationSearchDto.SearchTerm) ||
                    p.Abstract.Contains(paginationSearchDto.SearchTerm) || 
                    p.Keywords.Contains(paginationSearchDto.SearchTerm) ||
                    p.Publisher.Name.Contains(paginationSearchDto.SearchTerm));
        }
        else
        { 
            matchedPublications = _publications
                .Where(p =>
                    p.Title.Contains(paginationSearchDto.SearchTerm) ||
                    p.Title.StartsWith(paginationSearchDto.SearchTerm) ||
                    p.Title.FuzzyMatch(paginationSearchDto.SearchTerm, fuzzyMatchDistance) ||
                    p.Abstract.Contains(paginationSearchDto.SearchTerm) || 
                    p.Abstract.FuzzyMatch(paginationSearchDto.SearchTerm, fuzzyMatchDistance) ||
                    p.Keywords.Contains(paginationSearchDto.SearchTerm) ||
                    p.Publisher.Name.Contains(paginationSearchDto.SearchTerm));
        }
        
        
        IReadOnlyCollection<Publication> paginatedPublications = (await ApplyPagination(
            matchedPublications, paginationSearchDto)
            .ToListAsync())
            .AsReadOnly();
        
        return new PaginatedCollection<Publication>(
            Items: paginatedPublications,
            ResultCount: paginatedPublications.Count,
            TotalCount: await matchedPublications.CountAsync());
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
            ("year", true) => publications.OrderBy(p => p.Year),
            ("year", false) => publications.OrderByDescending(p => p.Year),
            ("lastmodified", true) => publications.OrderBy(p => p.LastModified),
            _ => publications.OrderByDescending(p => p.LastModified)
        };
    }
}