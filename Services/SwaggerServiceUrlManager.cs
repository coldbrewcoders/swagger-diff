using System;
using System.Collections.Generic;

namespace SwaggerDiff.Services
{
    public class SwaggerServiceUrlManager
    {
        public SwaggerServiceUrlManager() {
            _hostname = Environment.GetEnvironmentVariable("SWAGGER_DIFF_HOSTNAME");
            _apiVersion = Environment.GetEnvironmentVariable("SWAGGER_DIFF_API_VERSION");
        }

        private List<string> ServiceNames;
        private string _hostname;
        private string _apiVersion;

        public string GetUrl(string serviceName)
        {
            return $"{_hostname}/api/{serviceName}/swagger/{_apiVersion}/swagger.json";
        }

    }
    
}