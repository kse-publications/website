using Coravel.Invocable;

namespace Publications.API.BackgroundJobs;

public abstract class BaseRetriableTask<TTask>: IInvocable
    where TTask: BaseRetriableTask<TTask>
{
    private readonly int _maxRetries;
    private int _currentRetries;
    private readonly ILogger<TTask> _taskLogger;
    
    protected BaseRetriableTask(ILogger<TTask> taskLogger, int maxRetries)
    {
        _taskLogger = taskLogger;
        _maxRetries = maxRetries;
    }
    
    public async Task Invoke()
    {
        try
        {
            await ExecuteTask();
        }
        catch (Exception ex)
        {
            _taskLogger.LogError(ex, "An error occurred while executing the task.");

            if (_currentRetries < _maxRetries)
            {
                _currentRetries++;
                _taskLogger.LogWarning("Retrying task. Attempt {CurrentRetries} of {MaxRetries}.",
                    _currentRetries, _maxRetries);
                
                await Invoke();
            }
            else
            {
                throw;
            }
        }
    }
    
    protected abstract Task ExecuteTask();
}