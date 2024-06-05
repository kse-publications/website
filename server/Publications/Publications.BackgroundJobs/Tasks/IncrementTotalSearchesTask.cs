﻿using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using Publications.Application.Statistics;

namespace Publications.BackgroundJobs.Tasks;

public class IncrementTotalSearchesTask: IInvocable
{
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly ILogger<IncrementTotalSearchesTask> _logger;

    public static bool IsQueued { get; private set; }
    public static int SearchesCount { get; private set; }
    
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
        SearchesCount++;
    }
}