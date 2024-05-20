using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Publications.Application.Repositories;
using Publications.BackgroundJobs;
using Publications.Domain.Publications;

namespace Publications.API.Endpoints;

public static class SystemEndpoints
{
    public static IEndpointRouteBuilder MapSystemEndpoints(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapSyncEndpoint()
            .MapGetViewsEndpoint();
    }
    
    private static IEndpointRouteBuilder MapSyncEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/EB292BF0-E995-491A-A98E-6121601E1069/sync", 
            (ILogger<Program> logger, IScheduler scheduler) => 
            {
                logger.LogInformation("/sync endpoint hit");
                scheduler.Schedule<SyncDatabasesTask>()
                    .EverySecond()
                    .Once()
                    .PreventOverlapping(nameof(SyncDatabasesTask));
            });
        
        return endpoints;
    }
    
    private static IEndpointRouteBuilder MapGetViewsEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/FC2097AD-8BE2-463C-AD89-1190880C44AD/views", 
            async ([FromServices] IRequestsRepository requestsRepository) =>
            {
                Dictionary<int, int> views = await requestsRepository
                    .GetResourceViews<Publication>();
            
                return views;
            });
        
        return endpoints;
    }
}