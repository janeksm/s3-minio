namespace Integration.S3.Tests
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Xunit.DependencyInjection;
    using Xunit.DependencyInjection.Demystifier;
    using Xunit.DependencyInjection.Logging;

    public static class Startup
    {
        public static void ConfigureHost(IHostBuilder hostBuilder)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            hostBuilder.ConfigureHostConfiguration(builder => builder.AddConfiguration(config));

            EnsureTestRunningEnvironment();
        }

        public static void ConfigureServices(IServiceCollection services, HostBuilderContext builder)
        {
            var config = builder.Configuration;

            services.AddLogging(lb =>
            {
                lb.AddXunitOutput();
                lb.SetMinimumLevel(LogLevel.Information);
            });
            services.AddSingleton<IAsyncExceptionFilter, DemystifyExceptionFilter>();

            services.AddS3(config);
        }

        private static void EnsureTestRunningEnvironment()
        {
            string? env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env is null)
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
            }
        }
    }
}