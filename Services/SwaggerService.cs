using Microsoft.Extensions.Logging;

using SwaggerDiff.Models;

namespace SwaggerDiff.Services
{
    public class SwaggerService : ISwaggerService
    {
        private readonly SwaggerDiffContext _context;
        private readonly ILogger _logger;
        private readonly ISwaggerServiceUrlManager _urlManager;

        // Create instance of ClientRequestManager
        private IClientRequestManager _clientRequestManager;

        public SwaggerService(SwaggerDiffContext context, ILogger<SwaggerService> logger, ISwaggerServiceUrlManager urlManager, IClientRequestManager clientRequestManager)
        {
            _context = context;
            _logger = logger;
            _urlManager = urlManager;
            _clientRequestManager = clientRequestManager;
        }

        public async void Initialize()
        {
            _logger.LogInformation("Fetching JSON for services...");

            // Iterate over service list
            foreach (string serviceName in _urlManager.ServiceNames) 
            {
                // Find the JSON url for the service
                string requestUrl = _urlManager.GetUrl(serviceName);

                // Make async request to get the swagger JSON document
                string serviceJson = await _clientRequestManager.FetchServiceSwaggerJsonAsync(requestUrl);

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