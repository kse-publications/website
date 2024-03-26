
using Coravel.Invocable;

namespace Publications.API.BackgroundJobs;

public abstract class BaseRetriableTask<TTask>: IInvocable
    where TTask: BaseRetriableTask<TTask>
{
    private int _currentRetries;
    private readonly ILogger<TTask> _taskLogger;
    private readonly RetriableTaskOptions _options;
    
    protected BaseRetriableTask(ILogger<TTask> logger, RetriableTaskOptions options)
    {
        _taskLogger = logger;
        _options = options;
    }
    
    public async Task Invoke()
    {
        try
        {
            _taskLogger.LogInformation("Started executing {Task}...", typeof(TTask).Name);
            await DoBackgroundRetriableTaskAsync();
            _currentRetries = 0;
            _taskLogger.LogInformation("{Task} executed successfully.", typeof(TTask).Name);
        }
        catch (Exception ex)
        {
            _taskLogger.LogError(ex, "An error occurred while executing the {Task}.", typeof(TTask).Name);

            if (_currentRetries < _options.MaxRetries)
            {
                _currentRetries++;
                _taskLogger.LogWarning("Retrying task in {RetryDelay}. Attempt {CurrentRetries} of {MaxRetries}.",
                    _options.RetryDelay, _currentRetries, _options.MaxRetries);
                
                await Task.Delay(_options.RetryDelay);
                await Invoke();
            }
            else
            {
                throw;
            }
        }
    }
    
    protected abstract Task DoBackgroundRetriableTaskAsync();
}

public class RetriableTaskOptions
{
    public int MaxRetries { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);
}