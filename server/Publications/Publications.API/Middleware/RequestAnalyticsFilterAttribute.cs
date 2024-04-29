using System.Net;
using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Publications.BackgroundJobs;
using Publications.Domain.Requests;
using Publications.Domain.Shared;
using Publications.Infrastructure.Requests;

namespace Publications.API.Middleware;

public class RequestAnalyticsFilterAttribute<TResource> : TypeFilterAttribute 
    where TResource : Entity<TResource>
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

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await next();
            
            if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.OK &&
                ContainsClientUuid(context.HttpContext, out string? clientUuid))
            {
                string resourceName = ResourceHelper.GetResourceName<TResource>();
            
                // if the RequestAnalyticsFilter is used after the IdExtractionFilter in the filters pipeline
                // then the int id will be already extracted from the slug
                int resourceId = int.Parse(context.ActionArguments["id"]!.ToString()!);

                Request request = new(clientUuid!, resourceName, resourceId);
                _queue.QueueInvocableWithPayload<StoreRequestAnalyticsTask, Request>(request);
            }
            
            await next();
        }
        
        private static bool ContainsClientUuid(HttpContext context, out string? clientUuid)
        {
            clientUuid = null;
            
            if (context.Request.Headers.TryGetValue("client-uuid", out StringValues values) &&
                values.Count > 0)
            {
                clientUuid = values.First();
            }
            
            return clientUuid is not null;
        }
    }
}