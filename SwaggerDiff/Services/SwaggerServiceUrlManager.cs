using System.Collections.Generic;

namespace SwaggerDiff.Services
{
    public class SwaggerServiceUrlManager
    {
        public List<string> ServiceNames = new List<string>()
        {
            "applications",
            "auth",
            "distributionPartners",
            "documents",
            "messaging",
            "offering",
            "order",
            "users",
            "utility",
            "trades",
            "assets",
            "rten",
            "admin"
        };

        private string _hostname = "https://dev.templummarkets.com";

        private const string _apiVersion = "1";

        public string GetUrl(string serviceName)
        {
            return $"{_hostname}/api/{serviceName}/swagger/{_apiVersion}/swagger.json";
        }

    }
    
}