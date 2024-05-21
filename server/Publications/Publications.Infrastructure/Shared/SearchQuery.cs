using System.Text;
using Publications.Application.DTOs;
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

    public SearchQuery Filter(FilterDTO filterDTO)
    {
        return Filter(this, filterDTO);
    }
    
    public static SearchQuery CreateWithFilter(FilterDTO filterDTO)
    {
        return Filter(MatchAll(), filterDTO);
    }
    
    private static SearchQuery Filter(SearchQuery query, FilterDTO filterDTO)
    {
        if (filterDTO.GetParsedFilters().Length == 0)
            return query;

        SearchQuery fullQuery = query.AllowsAll() 
            ? query.Clear() 
            : query;
        
        SearchQuery filterGroupQuery = new();
        var filterGroups = filterDTO.GetParsedFilters()
            .Where(filters => filters.Length != 0);
        
        foreach (int[] filters in filterGroups)
        {
            filterGroupQuery._sb.Append(FilterQuery(filters.First()));
            foreach (var filter in filters.Skip(1))
            {
                filterGroupQuery.Or(FilterQuery(filter));
            }

            fullQuery.And(filterGroupQuery.Build());
            filterGroupQuery.Clear();
        }
        
        return fullQuery;
    }
    
    public SearchQuery Filter(FilterDTOV2 filterDTO)
    {
        return Filter(this, filterDTO);
    }
    
    public static SearchQuery CreateWithFilter(FilterDTOV2 filterDTO)
    {
        return Filter(MatchAll(), filterDTO);
    }
    
    private static SearchQuery Filter(SearchQuery query, FilterDTOV2 filterDTO)
    {
        if (filterDTO.GetParsedFilters().Keys.Count == 0)
            return query;

        SearchQuery fullQuery = query.AllowsAll() 
            ? query.Clear() 
            : query;
        
        SearchQuery filterGroupQuery = new();
        var filterGroups = filterDTO.GetParsedFilters().Values
            .Where(filters => filters.Length != 0);
        
        foreach (int[] filters in filterGroups)
        {
            filterGroupQuery._sb.Append(FilterQuery(filters.First()));
            foreach (var filter in filters.Skip(1))
            {
                filterGroupQuery.Or(FilterQuery(filter));
            }

            fullQuery.And(filterGroupQuery.Build());
            filterGroupQuery.Clear();
        }
        
        return fullQuery;
    }
    
    private static string FilterQuery(int filterId) =>
        new SearchFieldName($"{nameof(Publication.Filters)}_{nameof(Domain.Publications.Filter.Id)}")
            .EqualTo(filterId);

    public SearchQuery Search(
        string searchTerm, params SearchFieldName[] searchFields)
    {
        return Search(this, searchTerm, searchFields);
    }
    
    public static SearchQuery CreateWithSearch(
        string searchTerm, params SearchFieldName[] searchFields)
    {
        return Search(MatchAll(), searchTerm, searchFields);
    }

    private static SearchQuery Search(
        SearchQuery query, string searchTerm, params SearchFieldName[] searchFields)
    {
        const int minSearchTermLength = 2;
        if (searchTerm.Length < minSearchTermLength || searchFields.Length == 0)
            return query;
        
        SearchQuery searchBlockQuery = Where(searchFields.First().Prefix(searchTerm))
            .Or(searchFields.First().Search(searchTerm));
        
        foreach (SearchFieldName field in searchFields.Skip(1))
        {
            searchBlockQuery.Or(field.Prefix(searchTerm));
            searchBlockQuery.Or(field.Search(searchTerm));
        }
        
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