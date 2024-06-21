using Microsoft.Extensions.Logging;
using Publications.Application.Repositories;
using Publications.BackgroundJobs.Abstractions;
using Publications.Domain.Shared.Slugs;

namespace Publications.BackgroundJobs.Tasks;

public class VectorizePublicationsTask(
    ILogger<VectorizePublicationsTask> taskLogger,
    IPublicationsRepository publicationsRepository,
    IWordsService wordsService)
    : BaseLoggableTask<VectorizePublicationsTask>(taskLogger)
{
    private static DateTime? _startedCurrentRunAt;
    private static readonly TimeSpan LockTimeout = TimeSpan.FromHours(1);
    private static readonly object Lock = new();
    
    protected override async Task DoLoggedTaskAsync()
    {
        lock (Lock)
        {
            if (IsRunning())
            {
                taskLogger.LogInformation("Task is already running");
                return;
            }

            _startedCurrentRunAt = DateTime.UtcNow;
        }
        
        try
        {
            var publicationsToVectorize = await publicationsRepository.GetNonVectorizedAsync();

            foreach (var publication in publicationsToVectorize)
            {
                publication.Vectorize(wordsService);
            }

            await publicationsRepository.UpdateAsync(publicationsToVectorize);
            taskLogger.LogInformation("Vectorized publications: {totalCount}",
                publicationsToVectorize.Length);
        }
        finally
        {
            lock (Lock)
            {
                _startedCurrentRunAt = null;
            }
        }
    }
    
    private static bool IsRunning()
    {
        lock (Lock)
        {
            return _startedCurrentRunAt.HasValue && 
                   _startedCurrentRunAt.Value.Add(LockTimeout) > DateTime.UtcNow;
        }
    }
}