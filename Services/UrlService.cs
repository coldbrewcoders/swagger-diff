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
        public HashSet<string> ServiceNames { get; }

        // Slack webhook url
        public string SlackWebhookUrl { get; }

        public UrlService() 
        {
            /** Read environment variables that provide swagger documentation and slack message webhook URLs **/

            // Get hostname of swagger documentation URLs from environment variable
            _hostname = Environment.GetEnvironmentVariable("SWAGGER_DIFF_HOSTNAME");

            // Get optional port number of swagger documentation URL from environment variable (default port is 80)
            _port = Environment.GetEnvironmentVariable("SWAGGER_DIFF_PORT") ?? "80"; 

            // Get API version of all web services that we are monitoring 
            // NOTE: Currently only 1 API version supported, each monitored service must have same API version
            _apiVersion = Environment.GetEnvironmentVariable("SWAGGER_DIFF_API_VERSION");

            // Get list of web service names from environment variable (service names must be unique)
            ServiceNames = new HashSet<string>(Environment.GetEnvironmentVariable("SWAGGER_DIFF_SERVICENAMES").Split(","));

            // Webhook url from your slack integration that allows app to post messages
            SlackWebhookUrl = Environment.GetEnvironmentVariable("SWAGGER_DIFF_SLACK_WEBHOOK");
        }

        public bool IsValidServiceName(string serviceName)
        {
            // Check if string is one of the web services we are monitoring for documentation changes
            return ServiceNames.Contains(serviceName);
        }

        public string GetSwaggerDocumentUrl(string serviceName)
        {
            return $"{_hostname}:{_port}/api/{serviceName}/swagger/{_apiVersion}/swagger.json";
        }
    }
}