using Publications.Application.Services;
using Publications.Domain.Filters;
using Publications.Domain.Publications;

namespace Publications.Infrastructure.Services;

public class FiltersService: IFiltersService
{
    public async Task<IReadOnlyCollection<FilterGroup>> GetFiltersForPublicationsAsync(
        IEnumerable<Publication> publications)
    {
        List<Task<FilterGroup>> filtersTasks = Publication
            .GetEntityFilters()
            .Select(filter => Task.Run(() => GetFilterGroupFromProperty(
                publications, filter.PropertyName, filter.GroupId)))
            .ToList();

        return (await Task.WhenAll(filtersTasks))
            .OrderBy(f => f.Id)
            .ToList()
            .AsReadOnly();


        // return (await Task.WhenAll(
        //         Task.Run(() => GetYearsFilterGroup(publications)),
        //         Task.Run(() => GetTypesFilterGroup(publications)),
        //         Task.Run(() => GetLanguagesFilterGroup(publications))))
        //     .OrderBy(f => f.Id)
        //     .ToList()
        //     .AsReadOnly();
    }
    
    public async Task<IReadOnlyCollection<Publication>> AssignFiltersToPublicationsAsync(
        ICollection<Publication> publications, ICollection<FilterGroup> filters)
    {
        foreach (Publication publication in publications)
        {
            publication.Filters = filters
                .Select(fg => MatchingFilterFromGroup(publication, fg))
                .ToArray();
        }

        return publications
            .ToList()
            .AsReadOnly();
    }
    
    // private FilterGroup GetTypesFilterGroup(IEnumerable<Publication> publications) => 
    //     GetFilterGroupFromProperty(publications, p => p.Type, index: 1);
    //
    // private FilterGroup GetYearsFilterGroup(IEnumerable<Publication> publications) => 
    //     GetFilterGroupFromProperty(publications, p => p.Year, index: 2);
    //
    // private FilterGroup GetLanguagesFilterGroup(IEnumerable<Publication> publications) =>
    //     GetFilterGroupFromProperty(publications, p => p.Language, index: 3);
    
    private FilterGroup GetFilterGroupFromProperty(
        IEnumerable<Publication> publications, string propertyName, int index)
    {
        var filters = SelectPropertyByName(publications, propertyName)
            .Distinct()
            .Select(f => Filter.Create(f.ToString()!))
            .ToList();

        return new FilterGroup
        {
            Id = index,
            Name = propertyName,
            ResourceName = nameof(Publication),
            Filters = filters.ToArray()
        };
    }
    
    private static Filter MatchingFilterFromGroup(Publication publication, FilterGroup filterGroup)
    {
        return (filterGroup.Name switch
        {
            nameof(Publication.Year) => filterGroup.Filters
                .FirstOrDefault(f => f.Value == publication.Year.ToString()),
            
            nameof(Publication.Type) => filterGroup.Filters
                .FirstOrDefault(f => f.Value == publication.Type),
            
            nameof(Publication.Language) => filterGroup.Filters
                .FirstOrDefault(f => f.Value == publication.Language),
            
            _ => throw new ArgumentException("Invalid filter group name.")
        })!;
    }
    
    private static IEnumerable<object> SelectPropertyByName(
        IEnumerable<Publication> collection, string propertyName)
    {
        var propertyInfo = typeof(Publication).GetProperty(propertyName);

        return collection.Select(p => propertyInfo!.GetValue(p))!;
    }
}