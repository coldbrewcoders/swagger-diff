using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SwaggerDiff.Services.Interfaces;


namespace SwaggerDiff.Services
{
    public class DocumentationStoreService: IDocumentationStoreService
    {
        // Injected services
        private readonly ILogger _logger;

        // Thread-safe Dictionary for storing API documentation JSON files
        private readonly ConcurrentDictionary<string, string> _documentationStore = new ConcurrentDictionary<string, string>();


        public DocumentationStoreService(ILogger<DocumentationStoreService> logger)
        {
            // Init injected services
            _logger = logger;
        }


        public string GetValue(string webServiceName)
        {
            try {
                // Get Swagger documentation file from document store for a web-service
                return _documentationStore[webServiceName];
            }
            catch (KeyNotFoundException) {

                _logger.LogInformation($"No saved entry for web-service: ${webServiceName}. Attempting again to fetch initial API documentation for web-service...");

                // Return empty string so that controller knows to short-circuit
                return "";
            }
        }

        public void SetValue(string webServiceName, string documentationJson)
        {
            try 
            {
                // Add/Update Swagger documentation file for a web-service
                _documentationStore[webServiceName] = documentationJson;
            }
            catch (ArgumentNullException) 
            {
                _logger.LogInformation("Could not add file to document store for null key.");
            }
        }
    }
}

// TODO: Add indexer on this class for geting and seting values