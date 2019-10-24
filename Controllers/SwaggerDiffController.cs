using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SwaggerDiff.Models;
using SwaggerDiff.Services;

namespace SwaggerDiff.Controllers
{
    [Route("api/swaggerdiff")]
    [ApiController]
    public class SwaggerDiffController : ControllerBase
    {
        private readonly SwaggerDiffContext _context;
        private readonly ILogger _logger;
        private readonly IUrlService _urlService;
        private readonly IClientRequestService _clientRequestService;

        public SwaggerDiffController(SwaggerDiffContext context, ILogger<SwaggerDiffController> logger, IUrlService urlService, IClientRequestService clientRequestService)
        {
            _context = context;
            _logger = logger;
            _urlService = urlService;
            _clientRequestService = clientRequestService;
        }

        // GET: api/swaggerdiff
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SwaggerItem>>> GetSwaggerItems()
        {
            // Return every instance of SwaggerItem we have stored in the in-memory DB
            return await _context.SwaggerItems.ToListAsync();
        }

        // GET: api/swaggerdiff/:serviceName (Exposed Webhook URL)
        [HttpGet("{serviceName}")]
        [HttpPost("{serviceName}")]
        public async Task<ActionResult> GetSwaggerItem(string serviceName)
        {
            // Check if webhook was called with valid service name
            if(!_urlService.IsValidServiceName(serviceName))
            {
                return BadRequest($"Service name is not valid. Passed service name {serviceName}.");
            }

            // Get current instance of SwaggerItem for the corresponding service name
            SwaggerItem swaggerItem = await _context.SwaggerItems.FindAsync(serviceName);

            // Get currently stored serialized JSON document for document
            string previousJSON = swaggerItem.ServiceJSON;

            // Find URL to get new swagger JSON file
            string requestUrl = _urlService.GetUrl(serviceName);

            // Fetch fresh swagger JSON document for service
            string freshJSON = await _clientRequestService.FetchServiceSwaggerJsonAsync(requestUrl);

            // Check to see if there is a change

            // If there is a change...

                // Send slack notification of API change

                // Update DB with new service JSON file

                // Save updates to in-memory DB
                //await _context.SaveChangesAsync();
    
            return Ok();
        }
    }
}
