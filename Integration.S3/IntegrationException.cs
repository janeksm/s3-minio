namespace Integration.S3
{
    public class IntegrationException : Exception
    {
        private const string IntegrationName = "S3";
        private const string DefaultMessage = "error";

        public IntegrationException()
            : this(DefaultMessage)
        {
        }

        public IntegrationException(string message)
            : base(FormatMessage(IntegrationName, message))
        {
        }

        public IntegrationException(string message, Exception innerException)
            : base(FormatMessage(IntegrationName, message), innerException)
        {
        }

        private static string FormatMessage(string integrationName, string message)
        {
            return integrationName + ".Integration: " + message;
        }
    }
}