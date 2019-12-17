using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

// Service Interfaces
using SwaggerDiff.Services.Interfaces;


namespace SwaggerDiff.Services
{
    public class DocumentationStoreService: IDocumentationStoreService
    {
        private readonly ILogger _logger;

        // Thread-safe Dictionary for storing API documentation JSON files
        private readonly ConcurrentDictionary<string, string> _documentationStore = new ConcurrentDictionary<string, string>();


        // Constructor
        public DocumentationStoreService(ILogger<DocumentationStoreService> logger)
        {
            _logger = logger;
        }


        // Public Indexer
        public string this[string webServiceName]
        {
            get
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
            set
            {
                try 
                {
                    // Add/Update Swagger documentation file for a web-service
                    _documentationStore[webServiceName] = value;
                }
                catch (ArgumentNullException) 
                {
                    _logger.LogInformation("Could not add file to document store for null key.");
                }
            }
        }
    }
}