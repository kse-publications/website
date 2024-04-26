using System.Text;
using System.Text.RegularExpressions;
using StopWord;
using Unidecode.NET;

namespace Publications.API.Services;

public class SlugService
{
    private const char Separator = '-';
    
    public static string GenerateSlug(string name, string id, string language)
    {
        string cleanTransliteratedTitle = name
            .ToLower()
            .RemoveStopWords(language)
            .Unidecode();
        
        const int maxWords = 5;
        int wordsCount = 0;

        string[] words = cleanTransliteratedTitle
            .RemoveSpecialChars()
            .SplitWords();
    
        StringBuilder slug = new();
        foreach (string word in words)
        {
            if (wordsCount++ >= maxWords)
                break;
        
            slug.Append(word);
            slug.Append(Separator);
        }
    
        slug.Append(id);
        return slug.ToString();
    }
    
    public static string RetrieveIdFromSlug(string slug)
    {
        return slug
            .Split(Separator, StringSplitOptions.RemoveEmptyEntries)
            .Last();
    }
}

internal static class SlugExtensions
{
    internal static string RemoveSpecialChars(this string title)
    {
        return Regex.Replace(title, @"[^a-zA-Z0-9\s]", string.Empty);
    }
    
    internal static string[] SplitWords(this string title)
    {
        return title
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToArray();
    }
}