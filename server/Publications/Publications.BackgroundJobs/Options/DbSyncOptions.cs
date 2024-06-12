
namespace Publications.BackgroundJobs.Options;

public class DbSyncOptions
{
    public string Interval { get; set; } = null!;
    public bool Enabled { get; set; }
    public RetryOptions RetryOptions { get; set; } = null!;
}