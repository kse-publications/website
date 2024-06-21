using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Publications.Application.Services;

namespace Publications.BackgroundJobs;

public class ConfigurationHostedService: IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    
    public ConfigurationHostedService(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        var dbConfigurationService = scope.ServiceProvider.GetRequiredService<IDbConfigurationService>();
        await dbConfigurationService.ConfigureAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}