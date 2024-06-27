namespace Publications.Infrastructure.Shared;

public readonly struct SearchField
{
    private readonly string _name;
    
    public SearchField(string name)
    {
        _name = name;
    }
    
    public string Search(string searchTerm)
    {
        return $"(@{_name}:{searchTerm})";
    }

    public static string FullTextSearch(string word)
    {
        return $"({word})";
    }

    public string Prefix(string searchTerm)
    {
        return $"(@{_name}:{searchTerm}*)";
    }
    
    public static string PrefixSearch(string word)
    {
        return $"({word}*)";
    }
    
    public static string LevenshteinSearch(string word, int distance = 1)
    {
        return Enumerable.Repeat('%', distance).Aggregate(word,
            (current, next) => $"{next}{current}{next}");
    }
    
    public string EqualTo(string searchTerm)
    {
        return $"(@{_name}:{{{searchTerm}}})";
    }
    
    public string EqualTo(int numericValue)
    {
        return $"(@{_name}:[{numericValue},{numericValue}])";
    }
    
    public string NotEqualTo(string searchTerm)
    {
        return $"(-@{_name}:{{{searchTerm}}})";
    }
    
    public string NotEqualTo(int numericValue)
    {
        return $"(-@{_name}:[{numericValue},{numericValue}])";
    }
}