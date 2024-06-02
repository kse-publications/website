namespace Publications.Infrastructure.Shared;

public readonly struct SearchFieldName
{
    private readonly string _name;
    
    public SearchFieldName(string name)
    {
        _name = name;
    }
    
    public string Search(string searchTerm)
    {
        return $"(@{_name}:{searchTerm})";
    }

    public string Prefix(string searchTerm)
    {
        return $"(@{_name}:{searchTerm}*)";
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