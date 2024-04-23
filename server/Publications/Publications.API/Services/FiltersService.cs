using Publications.API.Models;

namespace Publications.API.Services;

public class FiltersService: IFiltersService
{
    public async Task<IReadOnlyCollection<FilterGroup>> GetFiltersForPublicationsAsync(
        IEnumerable<Publication> publications)
    {
        List<Task<FilterGroup>> filterGroupTasks = [];
        ((List<Func<IEnumerable<Publication>, FilterGroup>>)
        [
            GetYearsFilterGroup, 
            GetTypesFilterGroup, 
            GetLanguagesFilterGroup
        ]).ForEach(f =>
        {
            filterGroupTasks.Add(Task.Run(() => f(publications)));
        });
        
        List<FilterGroup> filters = (await Task.WhenAll(filterGroupTasks)).ToList();

        return AssignIdsToFilters(filters).AsReadOnly();
    }
    
    public async Task<ICollection<Publication>> AssignFiltersToPublicationsAsync(
        ICollection<Publication> publications, IReadOnlyCollection<FilterGroup> filters)
    {
        foreach (Publication publication in publications)
        {
            publication.Filters = filters
                .SelectMany(fg => MatchingFilters(publication, fg))
                .ToArray();
        }

        return publications;
    }

    private static List<Filter> MatchingFilters(Publication publication, FilterGroup filterGroup)
    {
        return filterGroup.Name switch
        {
            nameof(Publication.Year) => filterGroup.Filters
                .Where(f => f.Value == publication.Year.ToString()).ToList(),
            
            nameof(Publication.Type) => filterGroup.Filters
                .Where(f => f.Value == publication.Type).ToList(),
            
            nameof(Publication.Language) => filterGroup.Filters
                .Where(f => f.Value == publication.Language).ToList(),
            _ => []
        };
    }
    
    private List<FilterGroup> AssignIdsToFilters(List<FilterGroup> filters)
    {
        int filtersIdBase = 0;
        for (int i = 0; i < filters.Count; i++)
        {
            filters[i].Id = i + 1;
            for (int j = 0; j < filters[i].Filters.Length; j++)
            {
                filters[i].Filters[j].Id = filtersIdBase + j + 1;
            }

            filtersIdBase += filters[i].Filters.Length;
        }

        return filters;
    }

    private FilterGroup GetYearsFilterGroup(IEnumerable<Publication> publications)
    {
        var yearFilters = publications
            .Select(p => p.Year)
            .Distinct()
            .Select(y => new Filter { Value = y.ToString() })
            .ToList();

        return new FilterGroup
        {
            Name = nameof(Publication.Year),
            ResourceName = nameof(Publication),
            Filters = yearFilters.ToArray()
        };
    }
    
    private FilterGroup GetTypesFilterGroup(IEnumerable<Publication> publications)
    {
        var typeFilters = publications
            .Select(p => p.Type)
            .Distinct()
            .Select(t => new Filter { Value = t })
            .ToList();

        return new FilterGroup
        {
            Name =nameof(Publication.Type),
            ResourceName = nameof(Publication),
            Filters = typeFilters.ToArray()
        };
    }
    
    private FilterGroup GetLanguagesFilterGroup(IEnumerable<Publication> publications)
    {
        var languageFilters = publications
            .Select(p => p.Language)
            .Distinct()
            .Select(l => new Filter { Value = l })
            .ToList();

        return new FilterGroup
        {
            Name = nameof(Publication.Language),
            ResourceName = nameof(Publication),
            Filters = languageFilters.ToArray()
        };
    }
}