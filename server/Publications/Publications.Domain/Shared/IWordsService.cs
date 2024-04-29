using Publications.Domain.Shared.ValueObjects;

namespace Publications.Domain.Shared;

public interface IWordsService
{
    string RemoveStopWords(string text, IsoLanguageCode language);
    
    string Transliterate(string text);
}