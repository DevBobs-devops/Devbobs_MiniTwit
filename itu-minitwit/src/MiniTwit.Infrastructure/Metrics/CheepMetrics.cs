using System.Diagnostics.Metrics;
using Prometheus;
namespace Chirp.Infrastructure.Metrics;

public class CheepMetrics
{
    private static readonly Counter CheepsCreated = Prometheus.Metrics.CreateCounter(
        "cheeps_created_total",
        "Total number of cheeps created");

    public void RecordCheepCreated()
    {
        CheepsCreated.Inc();
    }
}

