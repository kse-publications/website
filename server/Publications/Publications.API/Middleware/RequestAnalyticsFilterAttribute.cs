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
            const string unknown = "Unknown";
        
            string sessionIdValue = context.HttpContext.Request.Cookies
                .TryGetValue("sessionId", out string? sessionId) 
                ? sessionId 
                : unknown;
        
            string[] pathSegments = context.HttpContext.Request.Path.Value?.Split(
                separator: '/', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        
            // if the RequestAnalyticsFilter is used after the IdExtractionFilter in the filters pipeline
            // then the numeric id will be already extracted from the slug
        
            int resourceId = context.ActionArguments.TryGetValue("id", out var argument)
                ? Convert.ToInt32(argument?.ToString() ?? pathSegments.Last())
                : 0;

            Request request = new(
                sessionIdValue,
                resourceName: pathSegments.First(),
                resourceId);
            
            _queue.QueueInvocableWithPayload<StoreRequestAnalyticsTask, Request>(request);
            
            await next();
        }
    }
}