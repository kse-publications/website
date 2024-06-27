using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Publications.BackgroundJobs.Tasks;

namespace Publications.API.Endpoints;

public static class SystemEndpoints
{
    public static IEndpointRouteBuilder MapSyncEndpoint(this IEndpointRouteBuilder endpoints,
        IConfiguration configuration)
    {
        endpoints.MapGet("/sync", (
            [FromServices]ILogger<Program> logger,
            [FromServices]IServiceProvider serviceProvider,
            [FromQuery] string key = "") => 
            {
                if (key != configuration["DbSync:Key"])
                {
                    logger.LogWarning("Unauthorized access to /sync endpoint");
                    return Results.Unauthorized();
                }
                
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
}