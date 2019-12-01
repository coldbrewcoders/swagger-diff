using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwaggerDiff.Models;
using SwaggerDiff.Services.Interfaces;


namespace SwaggerDiff.Controllers
{
    [Route("api/swaggerdiff")]
    [ApiController]
    public class SwaggerDiffController : ControllerBase
    {
        // Injected services
        private readonly ILogger _logger;
        private readonly IInitializationService _initializationService;
        private readonly IUrlService _urlService;
        private readonly IClientRequestService _clientRequestService;
        private readonly ICompareService _compareService;
        private readonly IDocumentStoreService _documentStoreService;


        public SwaggerDiffController(ILogger<SwaggerDiffController> logger, IInitializationService initializationService, IUrlService urlService, IClientRequestService clientRequestService, ICompareService compareService, IDocumentStoreService documentStoreService)
        {
            _logger = logger;
            _initializationService = initializationService;
            _urlService = urlService;
            _clientRequestService = clientRequestService;
            _compareService = compareService;
            _documentStoreService = documentStoreService;
        }

        // TODO: Remove this route
        // GET: api/swaggerdiff
        [HttpGet]
        public async Task<ActionResult> GetSwaggerItems()
        {
            // Get list of contents from the document store
            List<KeyValuePair<string, string>> documentStore = null;

            await Task.Run(() => {
                documentStore = _documentStoreService.GetDocumentStoreContents();
            });

            // Return all web-service documentation we currently have stored
            return Ok(documentStore);
        }

        // (GET | POST): api/swaggerdiff/:webServiceName (Exposed Webhook)
        [HttpGet("{webServiceName}")]
        [HttpPost("{webServiceName}")]
        public async Task<ActionResult> GetSwaggerItem(string webServiceName)
        {
            // Check if webhook was called with valid web-service name
            if (!_initializationService.IsValidWebServiceName(webServiceName))
            {
                // Create error response object
                ErrorObject errorObject = new ErrorObject("invalid_service_name", $"Web-Service name is not valid. Passed name: {webServiceName}.");

                // Return 400 response status with 
                return BadRequest(errorObject);
            }

            // Get currently stored serialized JSON document for web-service (keyed on service name)
            string previousJSON = _documentStoreService.GetValue(webServiceName);

            // Fetch fresh swagger JSON document for web-service
            string freshJSON;

            try 
            {
                // Attempt to get fresh JSON via client API request
                freshJSON = await _clientRequestService.FetchServiceSwaggerJsonAsync(webServiceName);
            }
            catch(HttpRequestException error) 
            {
                // Log client request error
                _logger.LogError($"Error fetching fresh Swagger documentation JSON file for '${webServiceName}', ${error}");

                // Return 500 status code
                return StatusCode(500);
            }

            // If documents are identical, short-circuit and do not perform diff checks
            if (string.Equals(previousJSON, freshJSON))
            {
                _logger.LogInformation("Previous and Fresh JSON files are identical, skipping additional checks.");
                return Ok();
            }

            // We now know that the documentation has been updated, perform full suite of diff checks
            await _compareService.CheckServiceForApiChanges(previousJSON, freshJSON);

            // Update document store with newest version of documentation for this web-service
            _documentStoreService.SetValue(webServiceName, freshJSON);

            // Return success status code
            return Ok();
        }
    }
}
