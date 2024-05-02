namespace Publications.Domain.Shared.ValueObjects;

public record IsoLanguageCode
{
    public string Value { get; init; }
    
    private IsoLanguageCode(string value)
    {
        Value = value;
    }
    
    public static IsoLanguageCode Create(string languageName)
    {
        return new IsoLanguageCode(languageName switch
        {
            nameof(German) => German,
            nameof(English) => English,
            nameof(Ukrainian) => Ukrainian,
            _ => English
        });
    }
    
    public static IsoLanguageCode German => new("de"); 
    public static IsoLanguageCode English => new("en");
    public static IsoLanguageCode Ukrainian => new("uk");
}