namespace Publications.API.Services;

public class LanguageService
{
    public static string German => "de";
    public static string English => "en";
    public static string Ukrainian => "uk";
    
    public static string GetTwoLetterIsoName(string language)
    {
        return language switch
        {
            nameof(German) => German,
            nameof(English) => English,
            nameof(Ukrainian) => Ukrainian,
            _ => English
        };
    }
}