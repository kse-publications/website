namespace Publications.API.BackgroundJobs;

public abstract class BaseBackgroundTask<TTask>: BackgroundService
    where TTask: BaseBackgroundTask<TTask>
{
    protected readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _interval;
    private readonly bool _runAtStartup;

    protected BaseBackgroundTask(
        IServiceProvider serviceProvider,
        TimeSpan interval,
        bool runAtStartup = true)
    {
        _serviceProvider = serviceProvider;
        _interval = interval;
        _runAtStartup = runAtStartup;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_runAtStartup)
        {
            await DoBackgroundTaskAsync(stoppingToken);
        }
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_interval, stoppingToken);
            await DoBackgroundTaskAsync(stoppingToken);
        }
    }
    
    protected abstract Task DoBackgroundTaskAsync(CancellationToken cancellationToken);
}