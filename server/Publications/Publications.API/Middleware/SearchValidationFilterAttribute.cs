using Microsoft.AspNetCore.Mvc.Filters;
using Publications.API.DTOs;

namespace Publications.API.Middleware;

public class SearchValidationFilterAttribute: ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next)
    {
        const string searchDtoKey = "paginationSearchDto";
        context.ActionArguments.TryGetValue(searchDtoKey, out object? paginationSearchDto);
        
        if (paginationSearchDto is PaginationSearchDTO searchDto)
        {
            context.ActionArguments[searchDtoKey] = searchDto with
            {
                SearchTerm = searchDto.CleanSearchTerm()
            };
        }
        
        await base.OnActionExecutionAsync(context, next);
    }
}