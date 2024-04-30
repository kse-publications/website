
using Microsoft.Extensions.Logging;

namespace Publications.BackgroundJobs.Abstractions;

public abstract class BaseRetriableTask<TTask>: BaseLoggableTask<TTask> 
    where TTask: BaseRetriableTask<TTask>
{
    private int _currentRetries;
    private readonly ILogger<TTask> _taskLogger;
    private readonly RetriableTaskOptions _options;
    
    protected BaseRetriableTask(ILogger<TTask> logger, RetriableTaskOptions options) : base(logger)
    {
        _taskLogger = logger;
        _options = options;
    }

    protected override async Task DoLoggedTaskAsync()
    {
        try
        {
            await DoRetriableTaskAsync();
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
                await DoLoggedTaskAsync();
            }
            else
            {
                throw;
            }
        }
    }

    protected abstract Task DoRetriableTaskAsync();
}