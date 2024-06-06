
namespace Publications.BackgroundJobs.Options;

public class DbSynchronizationOptions
{
    public string Interval { get; set; } = null!;
    public bool SyncEnabled { get; set; }
    public RetryOptions RetryOptions { get; set; } = null!;
    public ForceUpdateAll ForceUpdateAll { get; set; } = null!;
}

public class ForceUpdateAll
{
    public bool Enabled { get; set; }
    public bool RunOnlyOnce { get; set; }
}