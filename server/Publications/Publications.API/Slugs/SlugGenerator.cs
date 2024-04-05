using System.Text;
using System.Text.RegularExpressions;
using StopWord;
using Unidecode.NET;

namespace Publications.API.Slugs;

public class SlugGenerator
{
    public static string GenerateSlug(string name, string id, string language)
    {
        string cleanTransliteratedTitle = name
            .ToLower()
            .RemoveStopWords(language)
            .Unidecode();
        
        const int maxWords = 5;
        int wordsCount = 0;

        string[] words = SplitWords(RemoveSpecialChars(cleanTransliteratedTitle));
    
        StringBuilder slug = new();
        foreach (string word in words)
        {
            if (wordsCount++ >= maxWords)
                break;
        
            slug.Append(word);
            slug.Append('-');
        }
    
        slug.Append(id);
        return slug.ToString();
    }
    
    private static string RemoveSpecialChars(string title)
    {
        return Regex.Replace(title, @"[^a-zA-Z0-9\s]", string.Empty);
    }
    
    private static string[] SplitWords(string title)
    {
        return title.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToArray();
    }
    
    public static string RetrieveIdFromSlug(string slug)
    {
        string[] parts = slug.Split('-', StringSplitOptions.RemoveEmptyEntries);
        return parts.Last();
    }
}