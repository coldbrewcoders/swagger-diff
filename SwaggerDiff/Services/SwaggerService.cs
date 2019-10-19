using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

using SwaggerDiff.Models;

namespace SwaggerDiff.Services
{
    public class SwaggerService : ISwaggerService
    {
        private readonly SwaggerDiffContext _context;
        private readonly ILogger _logger;

        public SwaggerService(ILogger<SwaggerService> logger, SwaggerDiffContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void Initialize()
        {
            _logger.LogInformation("Fetching JSON for each service...");

            // Create list to store all swagger items
            List<SwaggerItem> swaggerItems = _context.SwaggerItems.ToList();

            // Create instance of SwaggerServiceUrlManager
            SwaggerServiceUrlManager urlManager = new SwaggerServiceUrlManager();

            _logger.LogInformation("Fetching JSON for services...");

            // Iterate over service list
            foreach(string serviceName in urlManager.ServiceNames) {

                // Find the JSON url for the service
                string requestUrl = urlManager.GetUrl(serviceName);

                _logger.LogInformation($"{serviceName}: {requestUrl}");

                // TODO: Make Sync request to get the swagger JSON document

                // TODO: Save JSON instead of URL
                SwaggerItem newEntry = new SwaggerItem(serviceName, requestUrl);

                swaggerItems.Add(newEntry);
            }

            _logger.LogInformation("Saving Swagger JSON documents to in-memory Database...");

            // Save all retrived service -> JSON pairs 
            _context.SaveChanges();

            _logger.LogInformation("Swagger Diff Initialization Complete.");
        }
    }
}