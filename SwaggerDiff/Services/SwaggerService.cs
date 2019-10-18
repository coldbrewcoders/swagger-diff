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
            _logger.LogInformation("Hello from Swagger Service");

            List<SwaggerItem> swaggerItems = _context.SwaggerItems.ToList();

            _logger.LogInformation(swaggerItems.ToString());
        
            // TODO: Execute the HTTP requests and add them to the in-memory DB
        }
    }
}