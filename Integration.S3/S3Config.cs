namespace Integration.S3
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    internal sealed class S3Config
    {
        public const string SectionName = "S3";

        public bool UseMock { get; set; }
        public string Uri { get; set; } = null!;
        public string AccessKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public bool UseSSL { get; set; }

        public Dictionary<string, string> ContentTypeMap { get; set; } = []; // Extension: ContentType

        public static S3Config GetAndConfigureForServices(IServiceCollection services, IConfiguration config)
        {
            var section = config.GetSection(SectionName);
            services.Configure<S3Config>(section);

            var s3Config = section.Get<S3Config>();
            Guard.Against.Null(s3Config);

            string? useMockFromEnv = Environment.GetEnvironmentVariable("S3_USE_MOCK");
            if (!string.IsNullOrEmpty(useMockFromEnv)
                && bool.TryParse(useMockFromEnv, out bool useMock))
            {
                s3Config.UseMock = useMock;
            }

            string? accessKeyFromEnv = Environment.GetEnvironmentVariable("S3_ACCESS_KEY");
            if (!string.IsNullOrEmpty(accessKeyFromEnv))
            {
                s3Config.AccessKey = accessKeyFromEnv;
            }

            string? secretKeyFromEnv = Environment.GetEnvironmentVariable("S3_SECRET_KEY");
            if (!string.IsNullOrEmpty(secretKeyFromEnv))
            {
                s3Config.SecretKey = secretKeyFromEnv;
            }

            return s3Config;
        }
    }
}