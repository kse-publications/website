using Coravel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Publications.Application;
using Publications.BackgroundJobs.Abstractions;

namespace Publications.BackgroundJobs;

public static class Installer
{
    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddHostedService<ConfigurationHostedService>();

        services
            .AddOptions<RetriableTaskOptions>()
            .Bind(configuration.GetSection("BackgroundTasks:SyncDatabasesTask"));
        
        services.AddTransient<SyncDatabasesTask>();
        services.AddScheduler();
        
        services.AddTransient<StoreRequestAnalyticsTask>();
        services.AddTransient<IncrementTotalSearchesTask>();
        services.AddQueue();
        return services;
    }
    
    public static void UseBackgroundJobs(this IServiceProvider serviceProvider, 
        IOptionsMonitor<FeatureFlags> optionsMonitor)
    {
        serviceProvider.UseScheduler(scheduler =>
        {
            scheduler.Schedule<SyncDatabasesTask>()
                .Hourly()
                .When(() => Task.FromResult(optionsMonitor.CurrentValue.SyncDatabases))
                .PreventOverlapping(nameof(SyncDatabasesTask));
        });
    }
}