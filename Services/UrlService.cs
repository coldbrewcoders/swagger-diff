using System;

namespace SwaggerDiff.Services
{
    public class UrlService : IUrlService
    {
        private readonly string _port;
        private readonly string _hostname;
        private readonly string _apiVersion;
        private readonly string[] _serviceNames;

        public UrlService() {
            string port = Environment.GetEnvironmentVariable("SWAGGER_DIFF_PORT");
            string hostname = Environment.GetEnvironmentVariable("SWAGGER_DIFF_HOSTNAME");
            string apiVersion = Environment.GetEnvironmentVariable("SWAGGER_DIFF_API_VERSION");
            string serviceNames = Environment.GetEnvironmentVariable("SWAGGER_DIFF_SERVICENAMES");

            _port = port;
            _hostname = hostname;
            _apiVersion = apiVersion;
            _serviceNames = serviceNames.Split(",");
        }

        public string[] GetServiceNames()
        {
            return _serviceNames;
        }

        public bool IsValidServiceName(string serviceName)
        {
            return Array.Exists(_serviceNames, name => name == serviceName);
        }

        public string GetUrl(string serviceName)
        {
            return $"{_hostname}:{_port}/api/{serviceName}/swagger/{_apiVersion}/swagger.json";
        }

    }
    
}