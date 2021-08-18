namespace Sample.Publisher.Console
{
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using Microsoft.Extensions.DependencyInjection;
    using Sample.Publisher.Console.HealthChecks;
    using UpBeats.HealthChecks.Publisher;
    using UpBeats.HealthChecks.Publisher.DependencyInjection;

    public static class HealthCheckConfig
    {
        public static void ConfigureHealthChecks(IServiceCollection services)
        {            
            var upbeatsOptions = new UpBeatsHealthCheckPublisherOptions
            {
                ApiKey = "{your api key}", 
                InstanceName = GetIpAddress(), // warn: wont work if multiple instances on same server
                ServiceName = typeof(Startup).Namespace,
                Version = typeof(Startup).Assembly.GetName().Version.ToString(),
            };

            ////services.Configure<HealthCheckPublisherOptions>(options =>
            ////{
            ////    options.Delay = TimeSpan.FromSeconds(2);
            ////    options.Period = TimeSpan.FromSeconds(30);
            ////});

            services.AddHealthChecks()
                .AddCheck<ExampleHealthCheck>("Example API", tags: ExampleHealthCheck.Tags)
                .AddCheck<ExampleHealthCheck>("Example Database", tags: Category.Database.AsTag())
                .AddCheck<ExampleHealthCheck>("Example Service", tags: Category.Service.AsTag())
                .AddCheck<ExampleHealthCheck>("Example Placeholder", tags: Category.Placeholder.AsTag())
                .AddUpBeatsPublisher(upbeatsOptions);            
        }

        private static string GetIpAddress() => Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork).ToString();
    }
}
