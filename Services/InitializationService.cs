using System.Collections.Concurrent;
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

        // Init thread-safe list of SwaggerItem instances
        private ConcurrentBag<SwaggerItem> _initialSwaggerItems = new ConcurrentBag<SwaggerItem>();

        public InitializationService(SwaggerDiffContext context, ILogger<InitializationService> logger, IUrlService urlService, IClientRequestService clientRequestService)
        {
            _context = context;
            _logger = logger;
            _urlService = urlService;
            _clientRequestService = clientRequestService;
        }

        public async Task Initialize()
        {
            _logger.LogInformation("Fetching JSON for all services...");

            // Iterate over all service names (in parallel)
            Parallel.ForEach(_urlService.ServiceNames, async (serviceName) => 
            {
                _logger.LogInformation($"Loading '{serviceName}' service JSON document");

                // Make async request to get the Swagger documentation JSON
                string serviceJson = await _clientRequestService.FetchServiceSwaggerJsonAsync(serviceName);

                // Create new entry for in-memory DB (keyed on unique servicename, stores serialized JSON)
                SwaggerItem newEntry = new SwaggerItem(serviceName, serviceJson);

                // Add new entry to a thread-safe list
                _initialSwaggerItems.Add(newEntry);
            });

            // Add new entry to in-memory DB
            await _context.SwaggerItems.AddRangeAsync(_initialSwaggerItems);

            // Save changes to in-memory DB
            await _context.SaveChangesAsync();

            _logger.LogInformation("Swagger-Diff initialization complete.");
        }
    }
}