using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Publications.API.BackgroundJobs;
using Publications.API.Models;
using Publications.API.Repositories.Requests;

namespace Publications.API.Extensions;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapSystemEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/EB292BF0-E995-491A-A98E-6121601E1069/sync", 
            (ILogger<Program> logger, IScheduler scheduler) => 
            {
                logger.LogInformation("/sync endpoint hit");
                scheduler.Schedule<SyncWithNotionBackgroundTask>()
                    .EverySecond()
                    .Once()
                    .PreventOverlapping(nameof(SyncWithNotionBackgroundTask));
            });

        endpoints.MapGet("/FC2097AD-8BE2-463C-AD89-1190880C44AD/views", 
            async ([FromServices] IRequestsRepository requestsRepository) =>
        {
            Dictionary<int, int> views = await requestsRepository
                .GetResourceViews<Publication>();
            
            return views;
        });
        
        endpoints.MapGet("/C178F906-9553-4EBF-AA3D-84A1C043F680/update-views", 
            (ILogger<Program> logger, IScheduler scheduler) =>
            {
                logger.LogInformation("/update-views endpoint hit");
                scheduler.Schedule<UpdateResourceViewsTask>()
                    .EverySecond()
                    .Once()
                    .PreventOverlapping(nameof(UpdateResourceViewsTask));
            });
        
        return endpoints;
    }
}