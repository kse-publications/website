using Coravel.Invocable;

namespace Publications.API.BackgroundJobs.Abstractions;

public abstract class BaseLoggableTask<TTask>: IInvocable 
    where TTask: BaseLoggableTask<TTask>
{
    private readonly ILogger<TTask> _taskLogger;

    protected BaseLoggableTask(ILogger<TTask> taskLogger)
    {
        _taskLogger = taskLogger;
    }

    public async Task Invoke()
    {
        _taskLogger.LogInformation("Started executing {Task}...", typeof(TTask).Name);
        await DoLoggedTaskAsync();
        _taskLogger.LogInformation("{Task} executed successfully.", typeof(TTask).Name);
    }

    protected abstract Task DoLoggedTaskAsync();
}