﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using UpBeats.ApiClient;

namespace UpBeats.HealthChecks.Publisher.DependencyInjection
{
    public static class UpBeatsHealthCheckBuilderExtensions
    {
        /// <summary>
        /// Add a health check publisher for UpBeats.
        /// </summary>
        /// <remarks>
        /// For each <see cref="HealthReport"/> published a custom service check indicating the health check status (OK - Healthy, WARNING - Degraded, CRITICAL - Unhealthy)
        /// and a metric indicating the total time the health check took to execute on milliseconds is sent to Datadog./>
        /// </remarks>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="options">Specifies configuration used to publish to UpBeats</param>
        /// <param name="defaultTags">Specifies a collection of tags to send with the custom check and metric.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
        public static IHealthChecksBuilder AddUpBeatsPublisher(this IHealthChecksBuilder builder, UpBeatsHealthCheckPublisherOptions options, string[] defaultTags = default)
        {
            builder.Services
                .AddSingleton<IHealthCheckPublisher>(sp =>
                {
                    var client = new UpBeatsApiClient(options.BaseUrl);
                    
                    return new UpBeatsHealthCheckPublisher(client, options);
                });

            return builder;
        }
    }
}
