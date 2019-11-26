using System;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using SwaggerDiff.Services.Interfaces;


namespace SwaggerDiff.Services
{
    public class InitializationService : IInitializationService
    {
        // Injected services
        private readonly ILogger _logger;
        private readonly IUrlService _urlService;
        private readonly IClientRequestService _clientRequestService;
        private readonly IDocumentStoreService _documentStoreService;

        // All web-service names (must be unique)
        private readonly HashSet<string> WebServiceNames;


        public InitializationService(IUrlService urlService, IClientRequestService clientRequestService, IDocumentStoreService documentStoreService, ILogger<InitializationService> logger)
        {
            // Init Services
            _logger = logger;
            _urlService = urlService;
            _clientRequestService = clientRequestService;
            _documentStoreService = documentStoreService;

            // Get list of web-service names from env variable (service names must be unique)
            WebServiceNames = new HashSet<string>(Environment.GetEnvironmentVariable("SWAGGER_DIFF_SERVICENAMES").Split(","));
        }

        public bool IsValidWebServiceName(string webServiceName)
        {
            // Check if string is one of the web services we are monitoring for documentation changes
            return WebServiceNames.Contains(webServiceName);
        }

        public void Initialize()
        {
            _logger.LogInformation("Fetching Swagger Documentation for all services...");

            // Iterate over all service names (in parallel)
            Parallel.ForEach(WebServiceNames, async (webServiceName) => 
            {
                _logger.LogInformation($"Loading documentation for web-service: '{webServiceName}' ");

                try
                {
                    // Make async request to get the Swagger documentation JSON for a web-service
                    string documentationJson = await _clientRequestService.FetchServiceSwaggerJsonAsync(webServiceName);

                    // Add recieved documentation to thread-safe key-value store
                    _documentStoreService.SetValue(webServiceName, documentationJson);

                }
                catch (HttpRequestException error)
                {
                    _logger.LogInformation($"Client request error occurred while fetching documentation for web-service: '{webServiceName}'. {error}");
                }
            });

            _logger.LogInformation("Successfully loaded documentation for all web-services.");
        }
    }
}