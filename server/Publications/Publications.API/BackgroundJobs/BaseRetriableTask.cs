
namespace Publications.API.BackgroundJobs;

public abstract class BaseRetriableTask<TTask>: BaseBackgroundTask<BaseRetriableTask<TTask>>
    where TTask: BaseRetriableTask<TTask>
{
    private readonly int _maxRetries;
    private TimeSpan _retryDelay;
    private int _currentRetries;
    private readonly ILogger<TTask> _taskLogger;
    
    protected BaseRetriableTask(
        ILogger<TTask> logger, 
        IServiceProvider serviceProvider,
        TimeSpan interval,
        int maxRetries, 
        TimeSpan retryDelay,
        bool runAtStartup = true): base(serviceProvider, interval, runAtStartup)
    {
        _taskLogger = logger;
        _maxRetries = maxRetries;
        _retryDelay = retryDelay;
    }
    
    protected override async Task DoBackgroundTaskAsync(CancellationToken cancellationToken)
    {
        try
        {
            await DoBackgroundRetriableTaskAsync();
            _taskLogger.LogInformation("{task} executed successfully.", nameof(TTask));
        }
        catch (Exception ex)
        {
            _taskLogger.LogError(ex, "An error occurred while executing the task.");

            if (_currentRetries < _maxRetries)
            {
                _currentRetries++;
                _taskLogger.LogWarning("Retrying task. Attempt {CurrentRetries} of {MaxRetries}.",
                    _currentRetries, _maxRetries);
                
                await DoBackgroundTaskAsync(cancellationToken);
            }
            else
            {
                throw;
            }
        }
    }
    
    protected abstract Task DoBackgroundRetriableTaskAsync();
}