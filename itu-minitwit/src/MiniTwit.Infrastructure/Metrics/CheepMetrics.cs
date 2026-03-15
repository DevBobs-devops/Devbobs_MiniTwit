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

    public static void RecordCheep(string? user)
    {
        CheepsCreated.Inc();

        if (user != null)
        {
            TotalCheepsPerUser.WithLabels(user).Inc();
        }
    }
}
