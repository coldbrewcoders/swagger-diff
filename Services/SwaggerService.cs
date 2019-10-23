using Microsoft.Extensions.Logging;

using SwaggerDiff.Models;

namespace SwaggerDiff.Services
{
    public class SwaggerService : ISwaggerService
    {
        private readonly SwaggerDiffContext _context;
        private readonly ILogger _logger;

        // Create instance of SwaggerServiceUrlManager
        private SwaggerServiceUrlManager urlManager = new SwaggerServiceUrlManager();

        // Create instance of ClientRequestManager
        private ClientRequestManager clientRequestManager = new ClientRequestManager();

        public SwaggerService(SwaggerDiffContext context, ILogger<SwaggerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async void Initialize()
        {
            _logger.LogInformation("Fetching JSON for services...");

            // Iterate over service list
            foreach(string serviceName in urlManager.ServiceNames) 
            {
                // Find the JSON url for the service
                string requestUrl = urlManager.GetUrl(serviceName);

                _logger.LogInformation($"{serviceName}: {requestUrl}");

                // Make async request to get the swagger JSON document
                string serviceJson = await clientRequestManager.FetchServiceSwaggerJsonAsync(serviceName);

                _logger.LogInformation($"{serviceName}: {serviceJson}");

                // TODO: Save JSON instead of URL
                // SwaggerItem newEntry = new SwaggerItem(serviceName, requestUrl);

                // Add new key, val pair of servicename -> Swagger JSON to in-memory DB
                // await _context.SwaggerItems.AddAsync(newEntry);
            }

            /* // Make Sync request to get the swagger JSON document
            clientRequestManager.FetchServiceSwaggerJsonAsync(requestUrl);

            string serviceName = Environment.GetEnvironmentVariable("SWAGGER_DIFF_SERVICENAME");

            // TODO: Save JSON instead of URL
            SwaggerItem newEntry = new SwaggerItem(serviceName, requestUrl);

            // Add new key, val pair of servicename -> Swagger JSON to in-memory DB
            await _context.SwaggerItems.AddAsync(newEntry);

            _logger.LogInformation("Saving Swagger JSON documents to in-memory Database...");

            // Save all in-memory DB changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Swagger Diff Initialization Complete."); */
        
        }
    }
}