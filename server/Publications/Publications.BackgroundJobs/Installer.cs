using Coravel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Publications.BackgroundJobs.Abstractions;

namespace Publications.BackgroundJobs;

public static class Installer
{
    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddHostedService<RedisHostedService>();

        services.Configure<RetriableTaskOptions>(
            configuration.GetSection("BackgroundTasks:SyncDatabasesTask"));
        
        services.AddTransient<SyncDatabasesTask>();
        services.AddScheduler();
        
        services.AddTransient<StoreRequestAnalyticsTask>();
        services.AddQueue();
        return services;
    }
    
    public static void UseBackgroundJobs(this IServiceProvider serviceProvider)
    {
        serviceProvider.UseScheduler(scheduler =>
        {
            scheduler.Schedule<SyncDatabasesTask>()
                .Hourly()
                .RunOnceAtStart()
                .PreventOverlapping(nameof(SyncDatabasesTask));
        });
    }
}