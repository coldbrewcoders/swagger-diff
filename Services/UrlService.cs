using System;


namespace SwaggerDiff.Services
{
    public class UrlService : IUrlService
    {
        // Swagger json document URL properties
        private readonly string _port;
        private readonly string _hostname;
        private readonly string _apiVersion;

        // All service names
        private readonly string[] _serviceNames;

        // Slack webhook url
        private readonly string _slackWebhookUrl;

        public UrlService() 
        {
            // Optional evn variable
            string port = Environment.GetEnvironmentVariable("SWAGGER_DIFF_PORT");

            // Use default port 80
            if (port == null) 
            {
                port = "80";
            }

            _port = port;

            // Required env variables
            _hostname = Environment.GetEnvironmentVariable("SWAGGER_DIFF_HOSTNAME");
            _apiVersion = Environment.GetEnvironmentVariable("SWAGGER_DIFF_API_VERSION");
            _serviceNames = Environment.GetEnvironmentVariable("SWAGGER_DIFF_SERVICENAMES").Split(",");
            _slackWebhookUrl = Environment.GetEnvironmentVariable("SWAGGER_DIFF_SLACK_WEBHOOK");
        }

        public string[] GetServiceNames()
        {
            return _serviceNames;
        }

        public bool IsValidServiceName(string serviceName)
        {
            return Array.Exists(_serviceNames, name => name == serviceName);
        }

        public string GetSwaggerDocumentUrl(string serviceName)
        {
            return $"{_hostname}:{_port}/api/{serviceName}/swagger/{_apiVersion}/swagger.json";
        }

        public string GetSlackWebhookUrl()
        {
            return _slackWebhookUrl;
        }
    }
}