using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using Publications.Application.Statistics;

namespace Publications.BackgroundJobs;

public class IncrementTotalSearchesTask: IInvocable
{
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly ILogger<IncrementTotalSearchesTask> _logger;

    public static bool IsQueued { get; private set; }
    public static int SearchesCount { get; private set; }
    
    public IncrementTotalSearchesTask(IStatisticsRepository statisticsRepository, ILogger<IncrementTotalSearchesTask> logger)
    {
        _statisticsRepository = statisticsRepository;
        _logger = logger;
    }

    public async Task Invoke()
    {
        await _statisticsRepository.IncrementTotalSearchesAsync(SearchesCount);
        IsQueued = false;
        SearchesCount = 0;
        _logger.LogInformation("Total searches count incremented.");
    }
    
    public static void Enqueue()
    {
        IsQueued = true;
    }
    
    public static void Increment()
    {
        SearchesCount++;
    }
}