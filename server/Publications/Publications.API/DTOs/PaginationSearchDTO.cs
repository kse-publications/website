using System.Text.RegularExpressions;

namespace Publications.API.DTOs;

public record PaginationSearchDTO(string SearchTerm = "") : PaginationDTO
{
    /// <summary>
    /// Checks if the search term is long enough and doesn't contain forbidden characters.
    /// </summary>
    /// <returns> True, if the search term can be used for fuzzy matching. </returns>
    public bool CanFuzzyMatch()
    {
        const int minFuzzyMatchLength = 4;
        const string forbiddenChars = "0123456789!@#$%^&*()_+{}|:\"<>?`~-=\\\\\\[\\\\];',./";
        
        return SearchTerm.Length >= minFuzzyMatchLength && 
               !SearchTerm.All(c => forbiddenChars.Contains(c));
    }
    
    /// <summary>
    /// Removes special characters from the search term and trims it.
    /// </summary>
    /// <returns> Sanitized search term. </returns>
    public string CleanSearchTerm()
    {
        const int maxSearchTermLength = 50;
        
        string sanitizedTerm = Regex.Replace(SearchTerm, @"[^a-zA-Z0-9_]", "");
        sanitizedTerm = Regex.Replace(sanitizedTerm, @"\s+", " ").Trim();
        
        return sanitizedTerm.Substring(0, Math.Min(maxSearchTermLength, sanitizedTerm.Length));
    }
}

