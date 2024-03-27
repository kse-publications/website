using Coravel.Scheduling.Schedule.Interfaces;
using Publications.API.BackgroundJobs;

namespace Publications.API.Extensions;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapSyncEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/EB292BF0-E995-491A-A98E-6121601E1069/sync", 
            (ILogger<Program> logger, IScheduler scheduler) => 
            {
                logger.LogInformation("/sync endpoint hit");
                scheduler.Schedule<SyncWithNotionBackgroundTask>()
                    .EverySecond()
                    .Once()
                    .PreventOverlapping(nameof(SyncWithNotionBackgroundTask));
            });
        
        return endpoints;
    }
}