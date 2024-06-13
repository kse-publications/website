using System.Net;
using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Publications.API.Serialization;
using Publications.BackgroundJobs;
using Publications.BackgroundJobs.Tasks;
using Publications.Domain.Requests;
using Publications.Domain.Shared;
using Publications.Infrastructure.Requests;

namespace Publications.API.Middleware;


public class RequestAnalyticsFilter<TResource> : IAsyncActionFilter
    where TResource : Entity
{
    private readonly IQueue _queue;

    public RequestAnalyticsFilter(IQueue queue)
    {
        _queue = queue;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await next();
        
        if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.OK &&
            ContainsClientUuid(context.HttpContext, out string? clientUuid))
        {
            string resourceName = ResourceHelper.GetResourceName<TResource>();

            if (context.ActionArguments["slug"] is SlugDTO slug)
            {
                int resourceId = slug.GetId();
                Request request = new(clientUuid!, resourceName, resourceId);
                _queue.QueueInvocableWithPayload<StoreRequestAnalyticsTask, Request>(request);
            }
        }
        
        await next();
    }
    
    private static bool ContainsClientUuid(HttpContext context, out string? clientUuid)
    {
        clientUuid = null;
        
        if (context.Request.Headers.TryGetValue("Client-Uuid", out StringValues values) &&
            values.Count > 0)
        {
            clientUuid = values.First();
        }
        
        return clientUuid is not null;
    }
}
