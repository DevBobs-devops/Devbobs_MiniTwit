namespace Chirp.Infrastructure.Metrics;
using Prometheus;

public class FollowMetrics
{
    private static readonly Gauge FollowersPerUser = Prometheus.Metrics.CreateGauge(
        "followers_per_user",
        "Number of followers for each user", 
        new GaugeConfiguration
        {
            LabelNames = new[] { "user" }
        });
    
    public static void IncrementFollower(string user)
    {
        FollowersPerUser.WithLabels(user).Inc();
    }
    
    public static void DecrementFollower(string user)
    {
        FollowersPerUser.WithLabels(user).Dec();
    }
}