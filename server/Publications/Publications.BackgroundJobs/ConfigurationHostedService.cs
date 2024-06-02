using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Publications.Application;
using Publications.Application.Services;

namespace Publications.BackgroundJobs;

public class ConfigurationHostedService: IHostedService
{
    private readonly IOptionsMonitor<FeatureFlags> _flagsMonitor;
    private readonly IServiceProvider _serviceProvider;
    
    public ConfigurationHostedService(
        IServiceProvider serviceProvider,
        IOptionsMonitor<FeatureFlags> flagsMonitor)
    {
        _serviceProvider = serviceProvider;
        _flagsMonitor = flagsMonitor;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_flagsMonitor.CurrentValue.SyncDatabases)
            return;

        using IServiceScope scope = _serviceProvider.CreateScope();
        var dbConfigurationService = scope.ServiceProvider.GetRequiredService<IDbConfigurationService>();
        var syncDatabasesTask = scope.ServiceProvider.GetRequiredService<SyncDatabasesTask>();
        
        await dbConfigurationService.ConfigureAsync();
        await syncDatabasesTask.Invoke();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}