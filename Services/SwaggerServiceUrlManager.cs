using System;

namespace SwaggerDiff.Services
{
    public class SwaggerServiceUrlManager
    {

        public SwaggerServiceUrlManager() {
            _hostname = Environment.GetEnvironmentVariable("SWAGGER_DIFF_HOSTNAME");
            _apiVersion = Environment.GetEnvironmentVariable("SWAGGER_DIFF_API_VERSION");
            _serviceName = Environment.GetEnvironmentVariable("SWAGGER_DIFF_SERVICENAME");
        }

        private string _hostname;
        private string _apiVersion;
        private string _serviceName;

        public string GetUrl()
        {
            return $"{_hostname}/api/{_serviceName}/swagger/{_apiVersion}/swagger.json";
        }

    }
    
}