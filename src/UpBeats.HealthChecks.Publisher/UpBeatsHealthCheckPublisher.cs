namespace UpBeats.HealthChecks.Publisher
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using UpBeats.ApiClient;

    public class UpBeatsHealthCheckPublisher : IHealthCheckPublisher
    {
        private readonly UpBeatsApiClient apiClient;
        private readonly UpBeatsHealthCheckPublisherOptions options;

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
            var healthCheck = HealthReportMapper.Map(report, this.options.InstanceName, this.options.ServiceName, this.options.Version);
            await this.apiClient.SubmitHealthCheckAsync(healthCheck, this.options.ApiKey);
        }
    }
}
