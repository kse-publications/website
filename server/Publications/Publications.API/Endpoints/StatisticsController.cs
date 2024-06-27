using Microsoft.AspNetCore.Mvc;
using Publications.Application.DTOs.Response;
using Publications.Application.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace Publications.API.Endpoints;

[ApiController]
[Route("stats")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsRepository _statisticsRepository;

    public StatisticsController(IStatisticsRepository statisticsRepository)
    {
        _statisticsRepository = statisticsRepository;
    }
    
    [HttpGet("overall")]
    [ProducesResponseType(typeof(OverallStats), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get overall statistics",
        Description = "Returns total publications and searches count."
    )]
    public async Task<IActionResult> GetOverallStats(CancellationToken cancellationToken)
    {
        OverallStats stats = await _statisticsRepository.GetOverallStatsAsync(cancellationToken);
        return Ok(stats);
    }
    
    [HttpGet("recent")]
    [ProducesResponseType(typeof(RecentStats), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get recent statistics",
        Description = "Returns recent views count and top recently viewed publications."
    )]
    public async Task<IActionResult> GetRecentStats(CancellationToken cancellationToken)
    {
        RecentStats stats = await _statisticsRepository.GetRecentStatsAsync(cancellationToken);
        return Ok(stats);
    }
}