using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Logging;

using SwaggerDiff.Models;

namespace SwaggerDiff.Services
{
    public class SwaggerService : ISwaggerService
    {
        private readonly ILogger _logger;
        private readonly SwaggerDiffContext _context;
        private readonly IHttpClientFactory _clientFactory;


        public SwaggerService(ILogger<SwaggerService> logger, SwaggerDiffContext context, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _context = context;
            _clientFactory = clientFactory;
        }

        public async void Initialize()
        {

            _logger.LogInformation("Hello from Swagger Service");

            List<SwaggerItem> swaggerItems = _context.SwaggerItems.ToList();

            _logger.LogInformation(swaggerItems.ToString());

            var client = _clientFactory.CreateClient();

            var response = await client.GetAsync("https://raw.githubusercontent.com/OAI/OpenAPI-Specification/master/examples/v2.0/json/petstore-simple.json");

            response.EnsureSuccessStatusCode();

            _logger.LogInformation($"JSON EXAMPLE:\n {response}");

            // TODO: Execute the HTTP requests and add them to the in-memory DB
        }
    }
}