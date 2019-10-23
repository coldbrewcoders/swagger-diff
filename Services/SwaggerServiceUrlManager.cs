using System;

namespace SwaggerDiff.Services
{
    public class SwaggerServiceUrlManager
    {
        public SwaggerServiceUrlManager() {
            string port = Environment.GetEnvironmentVariable("SWAGGER_DIFF_PORT");
            string hostname = Environment.GetEnvironmentVariable("SWAGGER_DIFF_HOSTNAME");
            string apiVersion = Environment.GetEnvironmentVariable("SWAGGER_DIFF_API_VERSION");
            string serviceNames = Environment.GetEnvironmentVariable("SWAGGER_DIFF_SERVICENAMES");

            _port = port;
            _hostname = hostname;
            _apiVersion = apiVersion;
            ServiceNames = serviceNames.Split(",");
        }

        private string _port;
        private string _hostname;
        private string _apiVersion;
        public readonly string[] ServiceNames;

        public string GetUrl(string serviceName)
        {
            if(Array.Exists(ServiceNames, name => name == serviceName))
            {
                return $"{_hostname}:{_port}/api/{serviceName}/swagger/{_apiVersion}/swagger.json";
            }
            else 
            {
                throw new ArgumentException("Parameter must be one of the configured service names.", "serviceName");
            } 
        }

    }
    
}