using System.Text;
using Publications.Application.DTOs;
using Publications.Domain.Publications;
using Publications.Domain.Shared;

namespace Publications.Infrastructure.Shared;

public class SearchQuery
{
    private readonly StringBuilder _sb = new();
    
    private const string OrOperator = " | ";
    private const char AndOperator = ' ';
    private const string AllowedAll = "*";

    public static SearchQuery Where(string query)
    {
        SearchQuery searchQuery = new();
        searchQuery._sb.Append(query);
        return searchQuery;
    }
    
    public SearchQuery Or(string query)
    {
        _sb.Insert(0, "(");
        _sb.Append(OrOperator);
        _sb.Append(query);
        _sb.Append(')');
        
        return this;
    }

    public SearchQuery And(string query)
    {
        _sb.Insert(0, "(");
        _sb.Append(AndOperator);
        _sb.Append(query);
        _sb.Append(')');
        
        return this;
    }

    public SearchQuery Filter(FilterDTO filterDTO)
    {
        return CreateWithFilter(filterDTO, this);
    }
    
    public static SearchQuery CreateWithFilter(FilterDTO filterDTO, SearchQuery? query = null)
    {
        if (filterDTO.GetParsedFilters().Length == 0 && (query is null || query.AllowsAll()))
            return Where(AllowedAll);

        SearchQuery fullQuery = query is not null 
            ? query.AllowsAll() 
                ? query.Clear() 
                : query 
            : new SearchQuery();
        
        SearchQuery filterGroupQuery = new();
        foreach (int[] filters in filterDTO.GetParsedFilters())
        {
            if (filters.Length == 0)
                continue;

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
        new SearchFieldName($"{nameof(Entity<Publication>.Filters)}_{nameof(Domain.Filters.Filter.Id)}")
            .EqualTo(filterId);
    
    private SearchQuery Clear()
    {
        _sb.Clear();
        return this;
    }
    
    public bool AllowsAll()
    {
        return _sb.ToString() == AllowedAll;
    }
    
    public string Build()
    {
        return _sb.ToString();
    }
}