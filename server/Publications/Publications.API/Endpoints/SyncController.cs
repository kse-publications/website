using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Publications.BackgroundJobs.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Publications.API.Endpoints;

[ApiController]
[Route("sync")]
public class SyncController: ControllerBase
{
    private readonly ILogger<SyncController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMutex _mutex;
    private readonly IServiceProvider _serviceProvider;

    public SyncController(
        IConfiguration configuration, 
        ILogger<SyncController> logger, 
        IMutex mutex, 
        IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _logger = logger;
        _mutex = mutex;
        _serviceProvider = serviceProvider;
    }

    [HttpGet]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Synchronize([FromQuery]string key = "")
    {
        if (key != _configuration["DbSync:Key"])
        {
            _logger.LogWarning("Unauthorized access to /sync endpoint");
            return Unauthorized();
        }
                
        _logger.LogInformation("/sync endpoint hit");
                
        _ = Task.Run(async () =>
        {
            await ExecuteSyncDatabasesTask();
        });
                
        return Ok();
    }

    [HttpGet("status")]
    [ProducesResponseType(typeof(SyncStatusResponse), StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Returns boolean indicating if the synchronization is currently running.")]
    public async Task<IActionResult> GetSyncStatus()
    {
        bool isRunning = !_mutex.TryGetLock(nameof(SyncDatabasesTask), timeoutMinutes: 0);
        
        return Ok(new SyncStatusResponse{ IsRunning = isRunning });
    }

    private async Task ExecuteSyncDatabasesTask()
    {
        if (!_mutex.TryGetLock(nameof(SyncDatabasesTask), timeoutMinutes: 30))
        {
            return;
        }
        
        var scope = _serviceProvider.CreateScope();
        var syncDatabasesTask = scope.ServiceProvider.GetRequiredService<SyncDatabasesTask>();
        
        try
        {
            await syncDatabasesTask.Invoke();
        }
        finally
        {
            _mutex.Release(nameof(SyncDatabasesTask));
        }
    }
}

public class SyncStatusResponse
{
    public bool IsRunning { get; set; }
}