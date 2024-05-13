
using System.Text.RegularExpressions;

namespace Publications.Application.DTOs;

/// <summary>
/// Basic pagination and search DTO.
/// </summary>
public partial record SearchDTO
{
    private readonly string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        init => _searchTerm = CleanSearchTerm(value);
    }
    
    private static string CleanSearchTerm(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return string.Empty;
        
        const int maxSearchTermLength = 50;
        
        string sanitizedTerm = ForbiddenCharactersRegex().Replace(searchTerm, "");
        sanitizedTerm = ExtraSpacesRegex().Replace(sanitizedTerm, " ").Trim();
        return sanitizedTerm.Substring(0, Math.Min(maxSearchTermLength, sanitizedTerm.Length));
    }
    
    [GeneratedRegex(@"[^a-zA-Z0-9_ ]")]
    private static partial Regex ForbiddenCharactersRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex ExtraSpacesRegex();
}

