namespace UpBeats.HealthChecks.Publisher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Newtonsoft.Json;
    using UpBeats.ApiClient.Model;

    public static class HealthReportMapper
    {
        public static SubmitHealthCheckRequestV1 Map(HealthReport report, string instanceName, string serviceName, string version)
        {
            var result = new SubmitHealthCheckRequestV1
            {
                Timestamp = DateTime.UtcNow,
                InstanceName = instanceName, 
                ServiceName = serviceName,
                ServiceVersion = version,
                Level = MapStatus(report.Status),
                Duration = (int?)report.TotalDuration.TotalMilliseconds,
            };

            foreach (KeyValuePair<string, HealthReportEntry> entry in report.Entries)
            {
                var probe = new HealthCheckResultProbeStatusV1
                {
                    ProbeName = entry.Key,
                    Level = MapStatus(entry.Value.Status),
                    Details = entry.Value.Description,
                    Duration = (int?)entry.Value.Duration.TotalMilliseconds,
                    ProbeType = entry.Key,
                    Category = GetCategory(entry.Value),
                    Message = entry.Value.Exception?.Message,
                    Metadata = MapMetadata(entry.Value),
                };
                result.Probes.Add(probe);
            }

            return result;
        }

        private static string GetCategory(HealthReportEntry entry)
        {
            var categoryTag = entry.Tags.FirstOrDefault(x => x.StartsWith("category:")) ?? "category:unknown";

            return categoryTag.Split(':')[1];
        }

        private static Dictionary<string, string> MapMetadata(HealthReportEntry entry)
        {
            if (entry.Data?.Any() == true)
            {
                return entry.Data.ToDictionary(x => x.Key, x => StringValue(x.Value));
            }

            return null;
        }

        private static string StringValue(object value)
        {
            try
            {
                return value.ToString();
            }
            catch
            {
                try
                {
                    return JsonConvert.SerializeObject(value);
                }
                catch(Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        private static SeverityLevel MapStatus(HealthStatus status)
        {
            switch (status)
            {
                case HealthStatus.Unhealthy:
                    return SeverityLevel.Fail;
                case HealthStatus.Degraded:
                    return SeverityLevel.Degraded;
                case HealthStatus.Healthy:
                    return SeverityLevel.Ok;
                default:
                    return SeverityLevel.Unknown;
            }
        }
    }
}