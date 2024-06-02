using System.ComponentModel.DataAnnotations;

namespace Publications.Application.DTOs;

public record FilterDTO
{
    /// <summary>
    /// Raw filters value from url query.
    /// </summary>
    [RegularExpression(@"^\d+:(?:\d+(?:-\d+)*)(?:;\d+:(?:\d+(?:-\d+)*))*;?$", ErrorMessage = "Invalid filters format.")]
    public string Filters { get; init; } = string.Empty;
    
    private Dictionary<int, int[]>? _filterIds;
    
    /// <summary>
    /// Parses the filters string into a dictionary of group ids and filter ids.
    /// </summary>
    /// <returns>Key: group id, Value: filter ids array</returns>
    public Dictionary<int, int[]> GetParsedFilters()
    {
        return _filterIds ??= ParseFilters(Filters);
    }

    public static FilterDTO CreateFromFilters(Dictionary<int, int[]> filterGroups)
    {
        return new FilterDTO { _filterIds = filterGroups };
    }
    
    private static Dictionary<int, int[]> ParseFilters(string rawFilters)
    {
        const char groupSeparator = ';';
        const char filterSeparator = '-';
        const char groupIdSeparator = ':';
        
        Dictionary<int, int[]> filterIds = [];
        string[] filterGroups = rawFilters.Split(
            groupSeparator, StringSplitOptions.RemoveEmptyEntries);

        foreach (var group in filterGroups)
        {
            string[] groupAndFilters = group.Split(groupIdSeparator);
            int groupId = int.Parse(groupAndFilters[0]);

            string[] filters = groupAndFilters[1].Split(filterSeparator);
            filterIds[groupId] = new int[filters.Length];

            for (int j = 0; j < filters.Length; j++)
            {
                filterIds[groupId][j] = int.Parse(filters[j]);
            }
        }

        return filterIds;
    }
}