using System.Net.Http;
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
        private readonly ICompareService _compareService;

        public SwaggerDiffController(SwaggerDiffContext context, ILogger<SwaggerDiffController> logger, IUrlService urlService, IClientRequestService clientRequestService, ICompareService compareService)
        {
            _context = context;
            _logger = logger;
            _urlService = urlService;
            _clientRequestService = clientRequestService;
            _compareService = compareService;
        }

        // GET: api/swaggerdiff
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SwaggerItem>>> GetSwaggerItems()
        {
            // Get current list of each monitored web service and its Swagger API documentation JSON
            List<SwaggerItem> swaggerItems = await _context.SwaggerItems.ToListAsync();

            // Return every instance of SwaggerItem we have stored in the in-memory DB
            return Ok(swaggerItems);
        }

        // GET: api/swaggerdiff/:serviceName (Exposed Webhook URL)
        [HttpGet("{serviceName}")]
        [HttpPost("{serviceName}")]
        public async Task<ActionResult> GetSwaggerItem(string serviceName)
        {
            // Check if webhook was called with valid service name
            if (!_urlService.IsValidServiceName(serviceName))
            {
                // Create error response object
                ErrorObject errorObject = new ErrorObject("invalid_service_name", $"Service name is not valid. Passed service name {serviceName}.");

                // Return 400 response status with 
                return BadRequest(errorObject);
            }

            // Get current instance of SwaggerItem for the corresponding service name
            SwaggerItem swaggerItem = await _context.SwaggerItems.FindAsync(serviceName);

            // Get currently stored serialized JSON document for service (keyed on service name)
            string previousJSON = swaggerItem.ServiceJSON;

            // Fetch fresh swagger JSON document for service
            string freshJSON;

            try 
            {
                // Attempt to get fresh JSON via client API request
                freshJSON = await _clientRequestService.FetchServiceSwaggerJsonAsync(serviceName);
            }
            catch(HttpRequestException error) {
                // Log client request error
                _logger.LogError($"Error fetching fresh Swagger documentation JSON file for '${serviceName}', ${error}");

                // Return 500 status code
                return StatusCode(500);
            }

            // Check if the fresh JSON document is identical to the previous one (using MD5 Hash comparison)
            // Note: this is a quick way to rule out any API documentation changes for this web service
            if (_compareService.AreJSONDocumentsIdentical(previousJSON, freshJSON))
            {
                _logger.LogInformation("Previous and Fresh JSON files are identical, skipping additional checks.");
                return Ok();
            }

            // We now know that the documentation has been updated, perform full suite of diff checks
            await _compareService.CheckServiceForApiChanges(previousJSON, freshJSON);

            // Update in-memory DB with the fresh JSON document for service name
            swaggerItem.ServiceJSON = freshJSON;

            // Save fresh serialized json to to in-memory DB
            await _context.SaveChangesAsync();

            // Return success status code
            return Ok();
        }
    }
}
