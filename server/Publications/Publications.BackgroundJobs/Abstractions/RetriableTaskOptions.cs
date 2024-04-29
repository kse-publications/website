namespace Publications.BackgroundJobs.Abstractions;

public class RetriableTaskOptions
{
    public int MaxRetries { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);
}