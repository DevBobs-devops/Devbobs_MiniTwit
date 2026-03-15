using Prometheus;

namespace Chirp.Infrastructure.Metrics;

public class AuthorMetrics
{
    private static readonly Counter TotalAuthors = Prometheus.Metrics.CreateCounter(
        "total_users",
        "Total number of users");
    
    public static void IncrementAuthors()
    {
        TotalAuthors.Inc();
    }

}