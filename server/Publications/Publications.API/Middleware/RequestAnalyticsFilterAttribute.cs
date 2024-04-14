using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
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
            if (ContainsClientUuid(context, out string? clientUuid))
            {
                string resourceName = (context.HttpContext.Request.Path.Value?
                        .Split('/', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>())
                    .First()
                    .ToLower(); 
            
                // if the RequestAnalyticsFilter is used after the IdExtractionFilter in the filters pipeline
                // then the numeric id will be already extracted from the slug
                int resourceId = int.Parse(context.ActionArguments["id"]!.ToString()!);

                Request request = new(clientUuid!, resourceName, resourceId);
                _queue.QueueInvocableWithPayload<StoreRequestAnalyticsTask, Request>(request);
            }
            
            await next();
        }
        
        private static bool ContainsClientUuid(ActionExecutingContext context, out string? clientUuid)
        {
            clientUuid = null;
            
            if (context.HttpContext.Request.Headers.TryGetValue("client-uuid", out StringValues values) &&
                values.Count > 0)
            {
                clientUuid = values.First();
            }
            
            return clientUuid is not null;
        }
    }
}