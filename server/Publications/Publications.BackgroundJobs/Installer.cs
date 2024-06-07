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
            .AddOptions<DbSyncOptions>()
            .Bind(configuration.GetSection("DbSync"));
        
        services.AddTransient<SyncDatabasesTask>();
        services.AddScheduler();
        
        services.AddTransient<StoreRequestAnalyticsTask>();
        services.AddTransient<IncrementTotalSearchesTask>();
        services.AddQueue();
        return services;
    }
    
    public static void UseBackgroundJobs(this IServiceProvider serviceProvider)
    {
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<DbSyncOptions>>();
        serviceProvider.UseScheduler(scheduler =>
        {
            scheduler.Schedule<SyncDatabasesTask>()
                .Cron(optionsMonitor.CurrentValue.Interval)
                .When(() => Task.FromResult(optionsMonitor.CurrentValue.Enabled))
                .PreventOverlapping(nameof(SyncDatabasesTask));
        });
    }
}