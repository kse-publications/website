using Publications.API.Models;
using Publications.API.Repositories;

namespace Publications.API.BackgroundJobs;

public class SyncWithNotionBackgroundTask: BaseRetriableTask<SyncWithNotionBackgroundTask>
{
    private readonly ILogger<SyncWithNotionBackgroundTask> _taskLogger;
    
    public SyncWithNotionBackgroundTask(
        ILogger<SyncWithNotionBackgroundTask> taskLogger,
        IServiceProvider serviceProvider,
        TimeSpan interval,
        int maxRetries,
        TimeSpan retryDelay,
        bool runAtStartup = true) 
        : base(taskLogger,serviceProvider, interval, maxRetries, retryDelay, runAtStartup)
    {
        _taskLogger = taskLogger;
    }

    protected override async Task DoBackgroundRetriableTaskAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var source = scope.ServiceProvider.GetRequiredService<IPublicationsSourceRepository>();
        var target = scope.ServiceProvider.GetRequiredService<IPublicationsRepository>();
        
        IReadOnlyCollection<Publication> publications = await source.GetPublicationsAsync();
        await target.InsertOrUpdateAsync(publications);
    }
}