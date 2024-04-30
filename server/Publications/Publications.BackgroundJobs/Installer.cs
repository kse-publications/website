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
            configuration.GetSection("BackgroundTasks:SyncWithNotion"));
        
        services.AddTransient<SyncWithNotionBackgroundTask>();
        services.AddScheduler();
        
        services.AddTransient<StoreRequestAnalyticsTask>();
        services.AddTransient<UpdateResourceViewsTask>();
        services.AddQueue();
        return services;
    }
    
    public static void UseBackgroundJobs(this IServiceProvider serviceProvider)
    {
        serviceProvider.UseScheduler(scheduler =>
        {
            scheduler.Schedule<SyncWithNotionBackgroundTask>()
                .Hourly()
                .RunOnceAtStart()
                .PreventOverlapping(nameof(SyncWithNotionBackgroundTask));
            
            scheduler.Schedule<UpdateResourceViewsTask>()
                .Cron("0 */2 * * *") // Every 2 hours
                .RunOnceAtStart()
                .PreventOverlapping(nameof(UpdateResourceViewsTask));
        });
    }
    
    public static bool AreScheduledJobsEnabled(this IConfiguration configuration)
    {
        return configuration.GetValue<string>("RUN_SCHEDULED_BG_JOBS") == "true";
    }
}