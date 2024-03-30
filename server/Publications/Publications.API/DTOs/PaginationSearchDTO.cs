using System.Text.RegularExpressions;

namespace Publications.API.DTOs;

public record PaginationSearchDTO(string SearchTerm = "") : PaginationFilterDTO
{
    /// <summary>
    /// Removes special characters from the search term and trims it.
    /// </summary>
    /// <returns> Sanitized search term. </returns>
    public string CleanSearchTerm()
    {
        const int maxSearchTermLength = 50;
        
        string sanitizedTerm = Regex.Replace(SearchTerm, @"[^a-zA-Z0-9_ ]", "");
        sanitizedTerm = Regex.Replace(sanitizedTerm, @"\s+", " ").Trim();
        return sanitizedTerm.Substring(0, Math.Min(maxSearchTermLength, sanitizedTerm.Length));
    }
}

