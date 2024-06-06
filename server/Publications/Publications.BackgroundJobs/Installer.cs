using Coravel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Publications.BackgroundJobs.Options;
using Publications.BackgroundJobs.Tasks;

namespace Publications.BackgroundJobs;

public static class Installer
{
    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddHostedService<ConfigurationHostedService>();

        services
            .AddOptions<DbSynchronizationOptions>()
            .Bind(configuration.GetSection("DbSynchronizationOptions"));
        
        services.AddTransient<SyncDatabasesTask>();
        services.AddScheduler();
        
        services.AddTransient<StoreRequestAnalyticsTask>();
        services.AddTransient<IncrementTotalSearchesTask>();
        services.AddQueue();
        return services;
    }
    
    public static void UseBackgroundJobs(this IServiceProvider serviceProvider)
    {
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<DbSynchronizationOptions>>();
        serviceProvider.UseScheduler(scheduler =>
        {
            scheduler.Schedule<SyncDatabasesTask>()
                .Cron(optionsMonitor.CurrentValue.Interval)
                .RunOnceAtStart()
                .When(() => Task.FromResult(optionsMonitor.CurrentValue.SyncEnabled))
                .PreventOverlapping(nameof(SyncDatabasesTask));
        });
    }
}