namespace UpBeats.HealthChecks.Publisher
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using UpBeats.ApiClient;

    public class UpBeatsHealthCheckPublisher : IHealthCheckPublisher
    {
        private readonly UpBeatsApiClient apiClient;
        private readonly UpBeatsHealthCheckPublisherOptions options;
        private static DateTime lastPublishDate = DateTime.MinValue;
        private static TimeSpan rateLimit = TimeSpan.FromSeconds(50);

        public UpBeatsHealthCheckPublisher(UpBeatsApiClient apiClient, UpBeatsHealthCheckPublisherOptions options)
        {
            this.apiClient = apiClient;
            this.options = options;
        }

        public UpBeatsHealthCheckPublisher(UpBeatsHealthCheckPublisherOptions options)
        {
            this.apiClient = new UpBeatsApiClient();
            this.options = options;
        }

        public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            if(lastPublishDate.Add(rateLimit) > DateTime.UtcNow)
            {
                return;
            }

            var healthCheck = HealthReportMapper.Map(report, this.options.InstanceName, this.options.ServiceName, this.options.Version);
            await this.apiClient.SubmitHealthCheckAsync(healthCheck, this.options.ApiKey);

            lastPublishDate = DateTime.UtcNow;
        }
    }
}
