namespace Sample.Publisher.Console
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args)
                .ConfigureServices((hostBuilderContext, services) =>
                {
                    var startup = new Startup(hostBuilderContext.Configuration);
                    startup.ConfigureServices(services);
                })
                .Build();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureAppConfiguration((hostContext, configurationBuilder) =>
                {
                    configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
                    configurationBuilder.AddJsonFile("appsettings.json", optional: true);
                    configurationBuilder.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName ?? "Production"}.json", optional: true, true);
                    configurationBuilder.AddEnvironmentVariables(prefix: string.Empty);
                    configurationBuilder.AddCommandLine(args);
                });
    }
}
