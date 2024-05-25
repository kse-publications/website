using System.Text;
using Publications.Domain.Shared.ValueObjects;

namespace Publications.Domain.Shared.Slugs;

public class SlugFactory
{
    private const int MaxWords = 5;
    private const char Separator = '-';
    
    public static string Create(
        string name, string id, IsoLanguageCode language, IWordsService wordsService)
    {
        string[] cleanTransliteratedWords = name
            .ToLower()
            .RemoveStopWords(language, wordsService)
            .Transliterate(wordsService)
            .RemoveSpecialChars()
            .SplitWords();
        
        StringBuilder slug = new();
        for (int i = 0; i < cleanTransliteratedWords.Length; i++)
        {
            if (i >= MaxWords)
                break;

            slug.Append(cleanTransliteratedWords[i]);
            slug.Append(Separator);
        }
        slug.Append(id);
        
        return slug.ToString();
    }
}