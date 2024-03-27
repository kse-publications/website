using System.Text.RegularExpressions;
using Publications.API.DTOs;
using Publications.API.Models;
using Publications.API.Repositories;
using Publications.API.Repositories.Abstractions;

namespace Publications.API.Services;

public class PublicationsService: IPublicationsService
{
    private readonly IPublicationsRepository _publicationsRepository;

    public PublicationsService(IPublicationsRepository publicationsRepository)
    {
        _publicationsRepository = publicationsRepository;
    }

    public async Task<PaginatedCollection<Publication>> GetAllAsync(
        PaginationDTO paginationDto, CancellationToken cancellationToken = default)
    {
        return await _publicationsRepository.GetAllAsync(paginationDto, cancellationToken);
    }

    public async Task<PaginatedCollection<Publication>> GetByFullTextSearchAsync(
        PaginationSearchDTO paginationSearchDto, 
        CancellationToken cancellationToken = default)
    {
        const int minSearchTermLength = 2;
        string sanitizeSearchTerm = SanitizeSearchTerm(paginationSearchDto.SearchTerm);
        
        if (sanitizeSearchTerm.Length < minSearchTermLength)
        {
            return new PaginatedCollection<Publication>(
                Items: new List<Publication>(),
                ResultCount: 0,
                TotalCount: 0);
        }
        
        return await _publicationsRepository.GetByFullTextSearchAsync(
            paginationSearchDto with { SearchTerm = sanitizeSearchTerm },
            cancellationToken);
    }

    public async Task<PaginatedCollection<Publication>> GetByAutoCompleteAsync(
        PaginationSearchDTO paginationSearchDto, 
        CancellationToken cancellationToken = default)
    {
        string sanitizeSearchTerm = SanitizeSearchTerm(paginationSearchDto.SearchTerm);
        if (string.IsNullOrWhiteSpace(sanitizeSearchTerm))
        {
            return new PaginatedCollection<Publication>(
                Items: new List<Publication>(),
                ResultCount: 0,
                TotalCount: 0);
        }
        
        return await _publicationsRepository.GetByAutoCompleteAsync(
            paginationSearchDto with { SearchTerm = sanitizeSearchTerm },
            AllowingFuzzyMatch(sanitizeSearchTerm),
            cancellationToken);
    }

    public async Task<Publication?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await _publicationsRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task InsertOrUpdateAsync(
        IEnumerable<Publication> publications,
        CancellationToken cancellationToken = default)
    {
        await _publicationsRepository.InsertOrUpdateAsync(
            publications, cancellationToken);
    }
    
    private static bool AllowingFuzzyMatch(string searchTerm)
    {
        const int minFuzzyMatchLength = 4;
        const string forbiddenChars = "0123456789!@#$%^&*()_+{}|:\"<>?`~-=\\\\\\[\\\\];',./";
        return searchTerm.Length >= minFuzzyMatchLength && 
               !searchTerm.All(c => forbiddenChars.Contains(c));
    }
    
    private static string SanitizeSearchTerm(string searchTerm)
    {
        string sanitizedTerm = Regex.Replace(searchTerm, "[\\~#%&*{}/:<>|\"-\\\\[\\\\]]", "");
        Console.WriteLine(sanitizedTerm);
        sanitizedTerm = Regex.Replace(sanitizedTerm, @"\s+", " ").Trim();
        return sanitizedTerm.Substring(0, Math.Min(50, sanitizedTerm.Length));
    }
}