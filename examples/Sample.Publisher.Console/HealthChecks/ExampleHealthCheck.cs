namespace Sample.Publisher.Console.HealthChecks
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using UpBeats.HealthChecks.Publisher;

    public class ExampleHealthCheck : IHealthCheck
    {
        public static string[] Tags => Category.Api.AsTag();

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var delay = new Random().Next(10, 500);
            await Task.Delay(delay, cancellationToken);

            var healthCheckResultHealthy = delay < 400;

            var data = new Dictionary<string, object>
            {
                {"meta1", Guid.NewGuid().ToString()},
                {"meta2", Guid.NewGuid().ToString()}
            };

            if (healthCheckResultHealthy)
            {
                return HealthCheckResult.Healthy("A healthy result.", data);
            }

            return HealthCheckResult.Unhealthy("An unhealthy result.", data: data);
        }
    }
}
