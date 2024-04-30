using System.Diagnostics;
using Coravel.Invocable;
using Microsoft.Extensions.Logging;

namespace Publications.BackgroundJobs.Abstractions;

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
        try
        {
            _taskLogger.LogInformation("Started executing {Task}...", typeof(TTask).Name);
            _stopwatch.Start();
            await DoLoggedTaskAsync();
            _stopwatch.Stop();
            _taskLogger.LogInformation("{Task} executed successfully. Elapsed time: {Elapsed}", 
                typeof(TTask).Name, _stopwatch.Elapsed);
        }
        catch (Exception e)
        {
            _taskLogger.LogError(e, "An error occurred while executing the {Task}.", typeof(TTask).Name);
            throw;
        }
    }

    protected abstract Task DoLoggedTaskAsync();
}