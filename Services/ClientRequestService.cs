using System.Net.Http;
using System.Threading.Tasks;
using SwaggerDiff.Services.Interfaces;


namespace SwaggerDiff.Services
{
    public class ClientRequestService : IClientRequestService
    {
        // Only instantiate one instance of HttpClient
        static readonly HttpClient httpClient = new HttpClient();
        
        private readonly IUrlService _urlService;

        public ClientRequestService(IUrlService urlService) {
            _urlService = urlService;
        }

        public async Task<string> FetchServiceSwaggerJsonAsync(string serviceName)
        {
            try 
            {
                // Get the URL for this web service's Swagger documentation JSON file
                string requestUrl = _urlService.GetSwaggerDocumentUrl(serviceName);

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
                // Let controller handle client request error
                throw error;
            }
        }

        // TODO: Write method that sends client request to slack webhook (results in slack notification)

    }
}
