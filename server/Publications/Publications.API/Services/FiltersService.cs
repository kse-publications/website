using System.Linq.Expressions;
using Publications.API.Models;

namespace Publications.API.Services;

public class FiltersService: IFiltersService
{
    public async Task<IReadOnlyCollection<FilterGroup>> GetFiltersForPublicationsAsync(
        IEnumerable<Publication> publications)
    {
        List<Func<IEnumerable<Publication>, FilterGroup>> filterFunctions =
        [
            GetYearsFilterGroup,
            GetTypesFilterGroup,
            GetLanguagesFilterGroup
        ];
        
        List<Task<FilterGroup>> filterTasks = filterFunctions
            .Select(f => Task.Run(() => f(publications)))
            .ToList();
        
        FilterGroup[] sortedFilters = (await Task.WhenAll(filterTasks))
            .OrderBy(f => f.Id)
            .ToArray();
        
        return AssignIdsToFilters(sortedFilters).AsReadOnly();
    }
    
    public async Task<ICollection<Publication>> AssignFiltersToPublicationsAsync(
        ICollection<Publication> publications, IReadOnlyCollection<FilterGroup> filters)
    {
        foreach (Publication publication in publications)
        {
            publication.Filters = filters
                .Select(fg => MatchingFilters(publication, fg))
                .ToArray();
        }

        return publications;
    }
    
    private FilterGroup[] AssignIdsToFilters(FilterGroup[] filters)
    {
        foreach (var filter in filters.SelectMany(group => group.Filters))
        {
            filter.Id = IdGenerator.GenerateFromValue(filter.Value);
        }

        return filters;
    }
    
    private FilterGroup GetTypesFilterGroup(IEnumerable<Publication> publications)
    {
        return GetFilterGroupFrom(publications, p => p.Type, index: 1);
    }
    
    private FilterGroup GetYearsFilterGroup(IEnumerable<Publication> publications)
    {
        return GetFilterGroupFrom(publications, p => p.Year, index: 2);
    }
    
    private FilterGroup GetLanguagesFilterGroup(IEnumerable<Publication> publications)
    {
        return GetFilterGroupFrom(publications, p => p.Language, index: 3);
    }
    
    private FilterGroup GetFilterGroupFrom<TProperty>(
        IEnumerable<Publication> publications, 
        Expression<Func<Publication, TProperty>> propertySelector,
        int index) where TProperty : notnull
    {
        var filters = publications
            .Select(propertySelector.Compile())
            .Distinct()
            .Select(f => new Filter { Value = f.ToString()! })
            .ToList();

        return new FilterGroup
        {
            Id = index,
            Name = GetPropertyName(propertySelector),
            ResourceName = nameof(Publication),
            Filters = filters.ToArray()
        };
    }
    
    private static Filter MatchingFilters(Publication publication, FilterGroup filterGroup)
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
    
    private static string GetPropertyName<TProperty>(
        Expression<Func<Publication, TProperty>> propertySelector)
    {
        if (propertySelector.Body is MemberExpression memberExpression)
            return memberExpression.Member.Name;
        
        if (propertySelector.Body is UnaryExpression unaryExpression)
            return ((MemberExpression)unaryExpression.Operand).Member.Name;
        
        throw new ArgumentException("Invalid property selector.");
    }
}