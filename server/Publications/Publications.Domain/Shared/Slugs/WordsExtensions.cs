using System.Text.RegularExpressions;
using Publications.Domain.Shared.ValueObjects;

namespace Publications.Domain.Shared.Slugs;

internal static class WordsExtensions
{
    internal static string Transliterate(this string words,
        IWordsService wordsService)
    {
        return wordsService.Transliterate(words);
    }
    
    internal static string RemoveStopWords(this string words,
        IsoLanguageCode language, IWordsService wordsService)
    {
        return wordsService.RemoveStopWords(words, language);
    }
    
    internal static string RemoveSpecialChars(this string words)
    {
        return Regex.Replace(words, @"[^a-zA-Z0-9 ]|\n|\r|\t|\f", string.Empty);
    }
    
    internal static string[] SplitWords(this string words)
    {
        return words
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToArray();
    }
}