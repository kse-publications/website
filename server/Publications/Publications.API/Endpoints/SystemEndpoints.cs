using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Publications.Application.Repositories;
using Publications.Application.Statistics;
using Publications.BackgroundJobs.Tasks;
using Publications.Domain.Publications;

namespace Publications.API.Endpoints;

public static class SystemEndpoints
{
    public static IEndpointRouteBuilder MapSystemEndpoints(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapSyncEndpoint()
            .MapGetViewsEndpoint()
            .MapGetOverallStatsEndpoint();
    }
    
    private static IEndpointRouteBuilder MapSyncEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/EB292BF0-E995-491A-A98E-6121601E1069/sync", 
            (ILogger<Program> logger, IServiceProvider serviceProvider) => 
            {
                logger.LogInformation("/sync endpoint hit");
                
                Task.Run(async () =>
                {
                    await ExecuteSyncDatabasesTask(serviceProvider);
                });
                
                return Results.Ok();
            });
        
        return endpoints;
    }

    private static async Task ExecuteSyncDatabasesTask(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var mutex = scope.ServiceProvider.GetRequiredService<IMutex>();

        if (!mutex.TryGetLock(nameof(SyncDatabasesTask), timeoutMinutes: 30))
        {
            return;
        }
        
        var syncDatabasesTask = scope.ServiceProvider.GetRequiredService<SyncDatabasesTask>();
        try
        {
            await syncDatabasesTask.Invoke();
        }
        finally
        {
            mutex.Release(nameof(SyncDatabasesTask));
        }
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

    private static IEndpointRouteBuilder MapGetOverallStatsEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/stats/overall", 
            async ([FromServices] IStatisticsRepository statisticsRepository) =>
            {
                var stats = await statisticsRepository.GetOverallStatsAsync();
                return stats;
            });
        
        return endpoints;
    }
}