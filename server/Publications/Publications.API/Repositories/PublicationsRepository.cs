using System.Linq.Expressions;
using System.Reflection;
using Publications.API.DTOs;
using Publications.API.Models;
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
        
        IRedisCollection<Publication> paginatedPublications = ApplyPagination(
            sortedPublications, paginationDto);
        
        return new PaginatedCollection<Publication>(
            Items: paginatedPublications.ToList(),
            Count: paginatedPublications.Count());
    }

    public async Task<PaginatedCollection<Publication>> GetByFullTextSearchAsync(
        PaginationSearchDTO paginationSearchDto,
        CancellationToken cancellationToken = default)
    {
        IRedisCollection<Publication> matchedPublications = _publications
            .Where(p => p.Title == paginationSearchDto.SearchTerm ||
                        p.Abstract == paginationSearchDto.SearchTerm ||
                        p.Keywords.Contains(paginationSearchDto.SearchTerm));
        
        IRedisCollection<Publication> paginatedPublications = ApplyPagination(
            _publications, paginationSearchDto);
        
        return new PaginatedCollection<Publication>(
            Items: paginatedPublications.ToList(),
            Count: paginatedPublications.Count());
    }

    public async Task<PaginatedCollection<Publication>> GetByAutoCompleteAsync(
        PaginationSearchDTO paginationSearchDto,
        CancellationToken cancellationToken = default)
    {
        IRedisCollection<Publication> matchedPublications = _publications
            .Where(p => p.Title.Contains(paginationSearchDto.SearchTerm));
        
        IRedisCollection<Publication> paginatedPublications = ApplyPagination(
            _publications, paginationSearchDto);
        
        return new PaginatedCollection<Publication>(
            Items: paginatedPublications.ToList(),
            Count: paginatedPublications.Count());
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
        PropertyInfo sortByProperty = (paginationDto.SortBy.ToLower() switch
        {
            "type" => typeof(Publication).GetProperty(nameof(Publication.Type)),
            "year" => typeof(Publication).GetProperty(nameof(Publication.Year)),
            "lastmodified" => typeof(Publication).GetProperty(nameof(Publication.LastModified)),
            _ => typeof(Publication).GetProperty(nameof(Publication.LastModified))
        })!;

        IRedisCollection<Publication> sortedPublications = Sort(
            publications,
            sortByExpression: publication => sortByProperty.GetValue(publication),
            paginationDto.Ascending);
        
        return sortedPublications;
    }
    
    private IRedisCollection<Publication> Sort<TKey>(
        IRedisCollection<Publication> publications,
        Expression<Func<Publication, TKey>> sortByExpression,
        bool ascending = false)
    {
        return ascending 
            ? publications.OrderBy(sortByExpression) 
            : publications.OrderByDescending(sortByExpression);
    }
}