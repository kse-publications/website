using System.ComponentModel.DataAnnotations;

namespace Publications.API.DTOs;

public record FilterDTO
{
    /// <summary>
    /// Raw filters value from url query.
    /// </summary>
    [RegularExpression(@"^\d+(?:-\d+)*(?:;\d+(?:-\d+)*)*;?$", ErrorMessage = "Invalid filters format.")]
    public string Filters { get; init; } = string.Empty;
    
    private int[][]? _filterIds;
    public int[][] GetParsedFilters()
    {
        return _filterIds ??= ParseFilters(Filters);
    }
    
    private static int[][] ParseFilters(string filters)
    {
        string[] filterGroups = filters.Split(';', StringSplitOptions.RemoveEmptyEntries);
        int[][] filterIds = new int[filterGroups.Length][];

        for (int i = 0; i < filterGroups.Length; i++)
        {
            string[] filterGroup = filterGroups[i].Split('-');
            filterIds[i] = new int[filterGroup.Length];

            for (int j = 0; j < filterGroup.Length; j++)
            {
                filterIds[i][j] = int.Parse(filterGroup[j]);
            }
        }

        return filterIds;
    }
}