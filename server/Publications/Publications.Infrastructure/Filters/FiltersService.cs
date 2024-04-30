using System.Linq.Expressions;
using Publications.Application.Services;
using Publications.Domain.Filters;
using Publications.Domain.Publications;

namespace Publications.Infrastructure.Filters;

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
        
        return (await Task.WhenAll(filterTasks))
            .OrderBy(f => f.Id)
            .ToList()
            .AsReadOnly();
    }
    
    public async Task<IReadOnlyCollection<Publication>> AssignFiltersToPublicationsAsync(
        ICollection<Publication> publications, ICollection<FilterGroup> filters)
    {
        foreach (Publication publication in publications)
        {
            publication.Filters = filters
                .Select(fg => MatchingFilters(publication, fg))
                .ToArray();
        }

        return publications
            .ToList()
            .AsReadOnly();
    }
    
    private FilterGroup GetTypesFilterGroup(IEnumerable<Publication> publications) => 
        GetFilterGroupFromProperty(publications, p => p.Type, index: 1);
    
    private FilterGroup GetYearsFilterGroup(IEnumerable<Publication> publications) => 
        GetFilterGroupFromProperty(publications, p => p.Year, index: 2);
    
    private FilterGroup GetLanguagesFilterGroup(IEnumerable<Publication> publications) =>
        GetFilterGroupFromProperty(publications, p => p.Language, index: 3);
    
    private FilterGroup GetFilterGroupFromProperty<TProperty>(
        IEnumerable<Publication> publications, 
        Expression<Func<Publication, TProperty>> propertySelector,
        int index) where TProperty : notnull
    {
        var filters = publications
            .Select(propertySelector.Compile())
            .Distinct()
            .Select(f => Filter.Create(f.ToString()!))
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