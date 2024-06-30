using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Publications.Application.DTOs.Request;
using Publications.BackgroundJobs.Tasks;

namespace Publications.API.Middleware;

public class SearchAnalyticsFilter: IAsyncActionFilter
{
    private readonly IQueue _queue;

    public SearchAnalyticsFilter(IQueue queue)
    {
        _queue = queue;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("paginationDTO", out var paginationDTO) && 
            paginationDTO is PaginationDTO { Page: 1 })
        {
            IncrementTotalSearchesTask.Increment();

            if (!IncrementTotalSearchesTask.IsQueued)
            {
                _queue.QueueInvocable<IncrementTotalSearchesTask>();
                IncrementTotalSearchesTask.Enqueue();
            }
        }
        
        await next();
    }
}