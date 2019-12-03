using System;
using SwaggerDiff.Services.Interfaces;


namespace SwaggerDiff.Services
{   
    public class UrlService : IUrlService
    {
        // Swagger json document URL properties
        private readonly string _port;
        private readonly string _hostname;
        private readonly string _apiVersion;

        // Slack webhook url
        private readonly string _slackWebhookUrl;


        public UrlService() 
        {
            // Get hostname of swagger documentation URLs from environment variable
            _hostname = Environment.GetEnvironmentVariable("SWAGGER_DIFF_HOSTNAME");

            // Get optional port number of swagger documentation URL from environment variable (default port is 80)
            _port = Environment.GetEnvironmentVariable("SWAGGER_DIFF_PORT") ?? "80"; 

            // Get API version of all web services that we are monitoring 
            // NOTE: Currently only 1 API version supported, each monitored service must have same API version
            _apiVersion = Environment.GetEnvironmentVariable("SWAGGER_DIFF_API_VERSION");

            // Webhook url from your slack integration that allows app to post messages
            _slackWebhookUrl = Environment.GetEnvironmentVariable("SWAGGER_DIFF_SLACK_WEBHOOK");
        }

        public string GetSwaggerDocumentUrl(string serviceName)
        {
            return $"{_hostname}:{_port}/api/{serviceName}/swagger/{_apiVersion}/swagger.json";
        }

        public string getSlackWebhookUrl()
        {
            return _slackWebhookUrl;
        }
    }
}