using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

using SwaggerDiff.Models;

namespace SwaggerDiff.Services
{
    public class InitializationService : IInitializationService
    {
        private readonly SwaggerDiffContext _context;
        private readonly ILogger _logger;
        private readonly IUrlService _urlService;
        private readonly IClientRequestService _clientRequestService;

        public InitializationService(SwaggerDiffContext context, ILogger<InitializationService> logger, IUrlService urlService, IClientRequestService clientRequestService)
        {
            _context = context;
            _logger = logger;
            _urlService = urlService;
            _clientRequestService = clientRequestService;
        }

        public async Task Initialize()
        {
            _logger.LogInformation("Fetching JSON for services...");

            // Iterate over all service names
            foreach (string serviceName in _urlService.GetServiceNames()) 
            {
                Console.WriteLine(serviceName);

                // Find the JSON url for the service
                string requestUrl = _urlService.GetUrl(serviceName);

                Console.WriteLine(requestUrl);

                // Make async request to get the swagger JSON document
                string serviceJson = await _clientRequestService.FetchServiceSwaggerJsonAsync(requestUrl);

                Console.WriteLine(serviceJson);

                // Save JSON instead of URL
                SwaggerItem newEntry = new SwaggerItem(serviceName, serviceJson);

                // Add new key, val pair of servicename -> Swagger JSON to in-memory DB
                await _context.SwaggerItems.AddAsync(newEntry);
            }

            _logger.LogInformation("Saving Swagger JSON documents to in-memory Database...");

            // Save all in-memory DB changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Swagger Diff Initialization Complete.");
        }
    }
}