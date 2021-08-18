using System;

namespace UpBeats.HealthChecks.Publisher
{
    public class UpBeatsHealthCheckPublisherOptions
    {
        public string ApiKey { get; set; }

        public string InstanceName { get; set; } = Guid.NewGuid().ToString("N");

        public string? ServiceName { get; set; }

        public string? Version { get; set; }

        public string BaseUrl { get; set; } = "https://api.upbeats.io";
    }
}