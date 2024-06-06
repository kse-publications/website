using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Publications.Application.Services;
using Publications.BackgroundJobs.Options;
using Publications.BackgroundJobs.Tasks;

namespace Publications.BackgroundJobs;

public class ConfigurationHostedService: IHostedService
{
    private readonly IOptions<DbSynchronizationOptions> _options;
    private readonly IServiceProvider _serviceProvider;
    
    public ConfigurationHostedService(
        IServiceProvider serviceProvider,
        IOptions<DbSynchronizationOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        var dbConfigurationService = scope.ServiceProvider.GetRequiredService<IDbConfigurationService>();
        await dbConfigurationService.ConfigureAsync();
        
        if(_options.Value.SyncEnabled)
        {
            var syncDatabasesTask = scope.ServiceProvider.GetRequiredService<SyncDatabasesTask>();
            await syncDatabasesTask.Invoke();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}