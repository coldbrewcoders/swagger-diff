using System;
using System.Collections.Generic;


namespace SwaggerDiff.Services
{
    public class UrlService : IUrlService
    {
        // Swagger json document URL properties
        private readonly string _port;
        private readonly string _hostname;
        private readonly string _apiVersion;

        // All service names
        private readonly HashSet<string> _serviceNames;

        // Slack webhook url
        public string SlackWebhookUrl { get; }

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
            _serviceNames = new HashSet<string>(Environment.GetEnvironmentVariable("SWAGGER_DIFF_SERVICENAMES").Split(","));
            SlackWebhookUrl = Environment.GetEnvironmentVariable("SWAGGER_DIFF_SLACK_WEBHOOK");
        }

        // TODO: fix
        public string[] GetServiceNames()
        {
            return _serviceNames;
        }

        // TODO: O(1) lookup 
        public bool IsValidServiceName(string serviceName)
        {
            return Array.Exists(_serviceNames, name => name == serviceName);
        }

        public string GetSwaggerDocumentUrl(string serviceName)
        {
            return $"{_hostname}:{_port}/api/{serviceName}/swagger/{_apiVersion}/swagger.json";
        }
    }
}