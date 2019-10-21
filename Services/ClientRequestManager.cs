using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace SwaggerDiff.Services
{
    public class ClientRequestManager : HttpClient
    {
        // Only instantiate one instance of HttpClient
        static readonly HttpClient client = new HttpClient();

        private readonly ILogger _logger;

        public ClientRequestManager(ILogger<SwaggerService> logger) 
        {
            _logger = logger;
        }

        public async void fetchServiceSwaggerJsonAsync(string requestUrl)
        {
            try 
            {
                // Invoke a get request to fetch swagger JSON document
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                // Ensure response code is a success type
                response.EnsureSuccessStatusCode();

                // Get the response body
                string responseBody = await response.Content.ReadAsStringAsync();

                // Output data received from request
                _logger.LogInformation(responseBody);
            }
            catch(HttpRequestException error)
            {
                _logger.LogError("Failed to fetch Swagger JSON document");	
                _logger.LogError($"Message :{error.Message}");
            }
        }
    }
}
