using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SwaggerDiff.Services.Interfaces;


namespace SwaggerDiff.Services
{
    public class DocumentStoreService: IDocumentStoreService
    {
        // Injected services
        private readonly ILogger _logger;

        // Thread-safe key-value pair for storing Swagger documentation files
        private readonly ConcurrentDictionary<string, string> _documentStore;


        public DocumentStoreService(ILogger<DocumentStoreService> logger)
        {
            // Init injected services
            _logger = logger;

            // init thread-safe document store
            _documentStore = new ConcurrentDictionary<string, string>();
        }

        public string GetValue(string webServiceName)
        {
            try {
                // Get Swagger documentation file from document store for a web-service
                return _documentStore[webServiceName];
            }
            catch (KeyNotFoundException) {
                _logger.LogInformation($"No saved entry for web-service: ${webServiceName}.");
                return "";
            }
        }

        public void SetValue(string webServiceName, string documentationJson)
        {
            try 
            {
                // Add/Update Swagger documentation file for a web-service
                _documentStore[webServiceName] = documentationJson;
            }
            catch (ArgumentNullException) 
            {
                _logger.LogInformation("Could not add file to document store for null key.");
            }
        }

        public List<KeyValuePair<string, string>> GetDocumentStoreContents()
        {
            // Create a list of key-value pairs
            var documentStoreContents = new List<KeyValuePair<string, string>>();

            // Iterate over keys in document store, create a list of key-value pairs
            foreach(string key in _documentStore.Keys)
            {
                documentStoreContents.Add(new KeyValuePair<string, string>(key, _documentStore[key]));
            }

            return documentStoreContents;
        }
    }
}