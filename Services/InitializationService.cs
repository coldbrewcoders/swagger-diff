using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

// Service Interfaces
using SwaggerDiff.Services.Interfaces;


namespace SwaggerDiff.Services
{
    public class InitializationService : IInitializationService
    {
        // Injected services
        private readonly ILogger _logger;
        private readonly IClientRequestService _clientRequestService;
        private readonly IDocumentationStoreService _documentationStoreService;

        // All web-service names (must be unique)
        private readonly HashSet<string> WebServiceNames;


        // Constructor
        public InitializationService(IClientRequestService clientRequestService, IDocumentationStoreService documentationStoreService, ILogger<InitializationService> logger)
        {
            // Init Services
            _logger = logger;
            _clientRequestService = clientRequestService;
            _documentationStoreService = documentationStoreService;

            // Get list of web-service names from env variable (service names must be unique)
            WebServiceNames = new HashSet<string>(Environment.GetEnvironmentVariable("SWAGGER_DIFF_SERVICENAMES").Split(","));
        }


        // Public methods
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

                    // Add received documentation to thread-safe key-value store
                    _documentationStoreService[webServiceName] = documentationJson;
                }
                catch (HttpRequestException error)
                {
                    _logger.LogInformation($"Client request error occurred while fetching documentation for web-service: '{webServiceName}'. {error}");
                }
            });

            _logger.LogInformation("Successfully loaded documentation for all web-services.");
        }

        public bool IsValidWebServiceName(string webServiceName)
        {
            // Check if string is one of the web services we are monitoring for documentation changes
            return WebServiceNames.Contains(webServiceName);
        }

        public async Task<bool> ReattemptDocumentFetch(string webServiceName)
        {
            if (IsValidWebServiceName(webServiceName))
            {
                // Attempt again to get API documentation for web-service
                string freshJSON = await _clientRequestService.FetchServiceSwaggerJsonAsync(webServiceName);

                // If we are successful in fetching API documentation..
                if (!string.Equals(freshJSON, string.Empty))
                {
                    // Put fresh API documentation JSON in document store
                    _documentationStoreService[webServiceName] = freshJSON;

                    // Successful reattempt to fetch previously unavailable JSON document
                    return true;
                }
            }

            // Unsuccessful reattempt
            return false;
        }
    }
}