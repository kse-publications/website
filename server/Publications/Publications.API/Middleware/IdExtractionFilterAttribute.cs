using Microsoft.AspNetCore.Mvc.Filters;
using Publications.API.Services;
using Publications.API.Slugs;

namespace Publications.API.Middleware;

public class IdExtractionFilterAttribute: ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("id", out var argument))
        {
            string slug = argument?.ToString() ?? string.Empty;
            string id = SlugGenerator.RetrieveIdFromSlug(slug);
            
            if (!int.TryParse(id, out int parsedId))
            {
                context.HttpContext.Response.StatusCode = 400;
                await context.HttpContext.Response.WriteAsync(
                    "Invalid ID provided in the request.");
                
                return;
            }

            context.ActionArguments["id"] = parsedId.ToString();
            
            await base.OnActionExecutionAsync(context, next);
        }
    }
}