using System.Text;
using Publications.Domain.Publications;


namespace Publications.Infrastructure.Shared;

public class SearchQuery
{
    private readonly StringBuilder _sb = new();
    
    private const string OrOperator = " | ";
    private const char AndOperator = ' ';
    private const string AllowedAll = "*";

    private SearchQuery() { }

    public static SearchQuery Where(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return MatchAll();
        
        SearchQuery searchQuery = new();
        searchQuery._sb.Append(query);
        return searchQuery;
    }
    
    private SearchQuery ReplaceWithQuery(string query)
    {
        _sb.Clear();
        _sb.Append(query);
        return this;
    }
    
    public SearchQuery Or(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return this;

        if (AllowsAll() || IsEmpty())
            return ReplaceWithQuery(query);
        
        _sb.Insert(0, "(");
        _sb.Append(OrOperator);
        _sb.Append(query);
        _sb.Append(')');
        
        return this;
    }

    public SearchQuery And(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return this;
        
        if (AllowsAll() || IsEmpty())
            return ReplaceWithQuery(query);
        
        _sb.Insert(0, "(");
        _sb.Append(AndOperator);
        _sb.Append(query);
        _sb.Append(')');
        
        return this;
    }
    
    public SearchQuery Filter(Dictionary<int, int[]> filters)
    {
        return Filter(this, filters);
    }
    
    public static SearchQuery CreateWithFilter(Dictionary<int, int[]> filters)
    {
        return Filter(MatchAll(), filters);
    }
    
    private static SearchQuery Filter(SearchQuery query, Dictionary<int, int[]> filters)
    {
        if (filters.Keys.Count == 0)
            return query;

        SearchQuery fullQuery = query.AllowsAll() 
            ? query.Clear() 
            : query;
        
        SearchQuery filterGroupQuery = new();
        var filterGroups = filters.Values
            .Where(group => group.Length != 0);
        
        foreach (int[] filtersGroup in filterGroups)
        {
            filterGroupQuery._sb.Append(FilterQuery(filtersGroup.First()));
            foreach (int filter in filtersGroup.Skip(1))
            {
                filterGroupQuery.Or(FilterQuery(filter));
            }

            fullQuery.And(filterGroupQuery.Build());
            filterGroupQuery.Clear();
        }
        
        return fullQuery;
    }
    
    private static string FilterQuery(int filterId) =>
        new SearchField($"{nameof(Publication.Filters)}_{nameof(Domain.Filters.Filter.Id)}")
            .EqualTo(filterId);

    public SearchQuery Search(string searchTerm)
    {
        return Search(this, searchTerm);
    }
    
    public static SearchQuery CreateWithSearch(string searchTerm)
    {
        return Search(MatchAll(), searchTerm);
    }

    private static SearchQuery Search(SearchQuery query, string searchTerm)
    {
        const int minSearchTermLength = 2;
        if (searchTerm.Length < minSearchTermLength)
            return query;
        
        SearchQuery searchBlockQuery = new();

        searchBlockQuery
            .Or(SearchField.FullTextSearch(searchTerm))
            .Or(SearchField.PrefixSearch(searchTerm));
        
        if (query.AllowsAll() || query.IsEmpty())
            return searchBlockQuery;
        
        query.And(searchBlockQuery.Build());

        return query;
    }
    
    public static SearchQuery MatchAll()
    {
        return Where(AllowedAll);
    }
    
    public bool AllowsAll()
    {
        return _sb.ToString() == AllowedAll;
    }
    
    public bool IsEmpty()
    {
        return _sb.Length == 0;
    }
    
    public string Build()
    {
        return _sb.ToString();
    }
    
    private SearchQuery Clear()
    {
        _sb.Clear();
        return this;
    }
}