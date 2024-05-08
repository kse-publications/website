using Microsoft.Extensions.Hosting;
using Publications.Application.Services;

namespace Publications.BackgroundJobs;

public class ConfigurationHostedService: IHostedService
{
    private readonly IDbConfigurationService _dbConfigurationService;
    
    public ConfigurationHostedService(IDbConfigurationService dbConfigurationService)
    {
        _dbConfigurationService = dbConfigurationService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _dbConfigurationService.ConfigureAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}