using System.Diagnostics;
using System.Diagnostics.Metrics;
using Prometheus;

namespace Chirp.Infrastructure.Metrics;

public class CheepMetrics
{
    private static readonly Counter CheepsCreated = Prometheus.Metrics.CreateCounter(
        "cheeps_created_total",
        "Total number of cheeps created"
    );

    private static readonly Counter TotalCheepsPerUser = Prometheus.Metrics.CreateCounter(
        "cheeps_per_user",
        "Number of cheeps by each user",
        new CounterConfiguration { LabelNames = new[] { "user" } }
    );

// https://oneuptime.com/blog/post/2026-01-25-prometheus-metrics-dotnet/view#histograms
    private static readonly Histogram DatabaseQueryDuration = Prometheus.Metrics.CreateHistogram(
        "database_query_duration_seconds",
        "Duration of database queries in seconds",
        new HistogramConfiguration
        {
            LabelNames = new[] { "operation", "table" },
            Buckets = new[] { 0.001, 0.005, 0.01, 0.05, 0.1, 0.5, 1.0, 5.0, 10.0 }
        });

    public static void RecordCheep(string? user)
    {
        CheepsCreated.Inc();

        if (user != null)
        {
            TotalCheepsPerUser.WithLabels(user).Inc();
        }
    }

    public static void RecordQueryGetCheeps(Stopwatch stopwatch)
    {
        DatabaseQueryDuration.Observe(stopwatch.Elapsed.TotalSeconds);
    }

    
}
