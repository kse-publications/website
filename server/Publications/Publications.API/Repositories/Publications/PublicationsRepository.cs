using Publications.API.DTOs;
using Publications.API.Models;
using Redis.OM;
using Redis.OM.Searching;

namespace Publications.API.Repositories.Publications;

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
        PaginationFilterDTO paginationFilterDTO, CancellationToken cancellationToken = default)
    {
        IRedisCollection<Publication> sortedFilteredPublications = _publications
            .ApplyFiltering(paginationFilterDTO)
            .ApplySorting(paginationFilterDTO.SortBy, paginationFilterDTO.Ascending);
        
        return await sortedFilteredPublications.ApplyPagination(
            paginationFilterDTO.Page,
            paginationFilterDTO.PageSize, 
            totalMatches: await sortedFilteredPublications.CountAsync());
    }

    public async Task<PaginatedCollection<Publication>> GetBySearchAsync(
        PaginationSearchDTO paginationSearchDTO,
        bool allowFuzzyMatch,
        CancellationToken cancellationToken = default)
    {
        const int fuzzyMatchDistance = 1;
        IRedisCollection<Publication> matchedPublications;

        if (!allowFuzzyMatch)
        {
            matchedPublications = _publications
                .Where(p =>
                    p.Title.Contains(paginationSearchDTO.SearchTerm) ||
                    p.Title.StartsWith(paginationSearchDTO.SearchTerm) ||
                    p.Abstract.Contains(paginationSearchDTO.SearchTerm) || 
                    p.Keywords.Contains(paginationSearchDTO.SearchTerm) ||
                    p.Publisher.Name.Contains(paginationSearchDTO.SearchTerm));
        }
        else
        { 
            matchedPublications = _publications
                .Where(p =>
                    p.Title.Contains(paginationSearchDTO.SearchTerm) ||
                    p.Title.StartsWith(paginationSearchDTO.SearchTerm) ||
                    p.Title.FuzzyMatch(paginationSearchDTO.SearchTerm, fuzzyMatchDistance) ||
                    p.Abstract.Contains(paginationSearchDTO.SearchTerm) || 
                    p.Abstract.FuzzyMatch(paginationSearchDTO.SearchTerm, fuzzyMatchDistance) ||
                    p.Keywords.Contains(paginationSearchDTO.SearchTerm) ||
                    p.Publisher.Name.Contains(paginationSearchDTO.SearchTerm));
        }
        
        return await matchedPublications.ApplyPagination(
            paginationSearchDTO.Page,
            paginationSearchDTO.PageSize, 
            totalMatches: await matchedPublications.CountAsync());
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
}