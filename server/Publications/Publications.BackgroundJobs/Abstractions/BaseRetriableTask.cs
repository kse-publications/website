
using Microsoft.Extensions.Logging;
using Publications.BackgroundJobs.Options;

namespace Publications.BackgroundJobs.Abstractions;

public abstract class BaseRetriableTask<TTask>: BaseLoggableTask<TTask> 
    where TTask: BaseRetriableTask<TTask>
{
    private int _currentRetries;
    private readonly ILogger<TTask> _taskLogger;
    private readonly RetryOptions _options;
    
    protected BaseRetriableTask(ILogger<TTask> logger, RetryOptions options) : base(logger)
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
                    _options.Delay, _currentRetries, _options.MaxRetries);
                
                await Task.Delay(_options.Delay);
                await DoLoggedTaskAsync();
            }
            else
            {
                throw;
            }
        }
        
        await OnSuccessAsync();
    }

    protected abstract Task DoRetriableTaskAsync();
    
    protected virtual Task OnSuccessAsync()
    {
        return Task.CompletedTask;
    }
}