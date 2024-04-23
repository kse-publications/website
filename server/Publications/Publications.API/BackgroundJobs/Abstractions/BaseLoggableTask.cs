using System.Diagnostics;
using Coravel.Invocable;

namespace Publications.API.BackgroundJobs.Abstractions;

public abstract class BaseLoggableTask<TTask>: IInvocable 
    where TTask: BaseLoggableTask<TTask>
{
    private readonly ILogger<TTask> _taskLogger;
    private readonly Stopwatch _stopwatch = new();

    protected BaseLoggableTask(ILogger<TTask> taskLogger)
    {
        _taskLogger = taskLogger;
    }

    public async Task Invoke()
    {
        _taskLogger.LogInformation("Started executing {Task}...", typeof(TTask).Name);
        _stopwatch.Start();
        await DoLoggedTaskAsync();
        _stopwatch.Stop();
        _taskLogger.LogInformation("{Task} executed successfully. Elapsed time: {Elapsed}", 
            typeof(TTask).Name, _stopwatch.Elapsed);
    }

    protected abstract Task DoLoggedTaskAsync();
}