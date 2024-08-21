namespace Integration.S3
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Minio;

    public static class S3Setup
    {
        public static void AddS3(this IServiceCollection services, IConfiguration config)
        {
            var s3Config = S3Config.GetAndConfigureForServices(services, config);

            if (s3Config.UseMock)
            {
                services.AddSingleton<IS3Client, S3ClientMock>();
                return;
            }

            services.AddMinio(
                configureClient => configureClient
                    .WithEndpoint(s3Config.Uri)
                    .WithCredentials(s3Config.AccessKey, s3Config.SecretKey)
                    .WithSSL(s3Config.UseSSL)
                    .Build(),
                ServiceLifetime.Singleton);

            services.AddSingleton<IS3Client, S3Client>();
        }
    }
}