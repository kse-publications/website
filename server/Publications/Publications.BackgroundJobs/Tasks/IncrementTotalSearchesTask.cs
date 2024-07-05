using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using Publications.Application.Repositories;

namespace Publications.BackgroundJobs.Tasks;

public class IncrementTotalSearchesTask: IInvocable
{
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly ILogger<IncrementTotalSearchesTask> _logger;
    private static int _searchesCount;

    public static bool IsQueued { get; private set; }

    public static int SearchesCount
    {
        get => _searchesCount;
        private set => _searchesCount = value;
    }

    public IncrementTotalSearchesTask(
        IStatisticsRepository statisticsRepository,
        ILogger<IncrementTotalSearchesTask> logger)
    {
        _statisticsRepository = statisticsRepository;
        _logger = logger;
    }

    public async Task Invoke()
    {
        try
        {
            await _statisticsRepository.IncrementTotalSearchesAsync(SearchesCount);
            IsQueued = false;
            SearchesCount = 0;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to increment total searches.");
            throw;
        }
    }
    
    public static void Enqueue()
    {
        IsQueued = true;
    }
    
    public static void Increment()
    {
        Interlocked.Increment(ref _searchesCount);
    }
}