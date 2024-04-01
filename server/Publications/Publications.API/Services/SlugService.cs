using System.Text;
using System.Text.RegularExpressions;

namespace Publications.API.Services;

public class SlugService
{
    public static string GenerateSlug(string title, string id)
    {
        string[] escapeWords = 
        ["a", "an", "the", "and", "but", "or", "for", 
            "nor", "on", "at", "to", "from", "by", "of"];
        
        const int maxWords = 5;
        int wordsCount = 0;
    
        string cleanedTitle = Regex.Replace(title, @"[^a-zA-Z0-9\s]", string.Empty);
        string[] words = cleanedTitle
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.ToLower())
            .ToArray();
    
        StringBuilder slug = new();
        foreach (string word in words)
        {
            if (escapeWords.Contains(word))
                continue;

            if (wordsCount++ >= maxWords)
                break;
        
            slug.Append(word);
            slug.Append('-');
        }
    
        slug.Append(id);
        return slug.ToString();
    }
    
    public static string RetrieveIdFromSlug(string slug)
    {
        string[] parts = slug.Split('-', StringSplitOptions.RemoveEmptyEntries);
        return parts.Last();
    }
}