using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Publications.API.BackgroundJobs;
using Publications.API.Models;

namespace Publications.API.Middleware;

public class RequestAnalyticsFilterAttribute : TypeFilterAttribute
{        
    public RequestAnalyticsFilterAttribute() : base(typeof(RequestAnalyticsFilterImplementation))
    {
    }

    private class RequestAnalyticsFilterImplementation : IAsyncActionFilter
    {
        private readonly IQueue _queue;

        public RequestAnalyticsFilterImplementation(IQueue queue)
        {
            _queue = queue;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Cookies.TryGetValue("client-uuid", out string? sessionId))
            {
                string resourceName = (context.HttpContext.Request.Path.Value?.Split(
                        separator: '/', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>())
                    .First().ToLower(); 
            
                // if the RequestAnalyticsFilter is used after the IdExtractionFilter in the filters pipeline
                // then the numeric id will be already extracted from the slug
                int resourceId = int.Parse(context.ActionArguments["id"]!.ToString()!);

                Request request = new(sessionId, resourceName, resourceId);
                _queue.QueueInvocableWithPayload<StoreRequestAnalyticsTask, Request>(request);
            }
            
            await next();
        }
    }
}