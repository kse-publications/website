using Publications.Domain.Shared;
using Publications.Domain.Shared.Slugs;
using Publications.Domain.Shared.ValueObjects;
using StopWord;
using Unidecode.NET;

namespace Publications.Infrastructure.Services;

public class WordsService: IWordsService
{
    public string RemoveStopWords(string text, IsoLanguageCode language)
    {
        return text.RemoveStopWords(language.Value);
    }

    public string Transliterate(string text)
    {
        return text.Unidecode();
    }
}