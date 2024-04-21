using System.Text.RegularExpressions;

namespace Publications.API.DTOs;

/// <summary>
/// Provides extension methods for classes implementing <see cref="ISearchDTO"/>.
/// </summary>
public static partial class SearchDTOExtensions
{
    public static string CleanSearchTerm(this ISearchDTO searchDto, string? searchTerm)
    {
        return CleanSearchTerm(searchTerm);
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