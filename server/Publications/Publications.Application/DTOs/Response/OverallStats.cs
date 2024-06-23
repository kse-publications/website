
namespace Publications.Application.DTOs.Response;

public class OverallStats
{
    public int TotalPublicationsCount { get; init; }
    public long TotalSearchesCount { get; init; }
    public int TotalViewsCount { get; init; }

    public OverallStats(
        int totalPublicationsCount,
        long totalSearchesCount,
        int totalViewsCount)
    {
        TotalPublicationsCount = totalPublicationsCount;
        TotalSearchesCount = totalSearchesCount;
        TotalViewsCount = totalViewsCount;
    }
}