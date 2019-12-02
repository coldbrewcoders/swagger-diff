using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SwaggerDiff.Services.Interfaces;


namespace SwaggerDiff.Services
{
    public class ClientRequestService : IClientRequestService
    {
        // Only instantiate one instance of HttpClient
        static readonly HttpClient httpClient = new HttpClient();
        
        // Injected services
        private readonly IUrlService _urlService;
        private readonly ILogger _logger;


        public ClientRequestService(ILogger<IClientRequestService> logger, IUrlService urlService) {
            _urlService = urlService;
            _logger = logger;
        }


        public async Task<string> FetchServiceSwaggerJsonAsync(string webServiceName)
        {
            try 
            {
                // Get the URL for this web service's Swagger documentation JSON file
                string requestUrl = _urlService.GetSwaggerDocumentUrl(webServiceName);

                // Invoke a GET request to get JSON file
                HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

                // Validate request received a successful status code
                response.EnsureSuccessStatusCode();

                // Get the response content 
                string responseBody = await response.Content.ReadAsStringAsync();

                // Return response body as string
                return responseBody;
            }
            catch (HttpRequestException error)
            {
                _logger.LogInformation($"Client request error occurred while fetching documentation for web-service: '{webServiceName}'. {error}");
                return "";
            }
        }



        // TODO: Write method that sends client request to slack webhook (results in slack notification)

    }
}
