using System;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SwaggerDiff.Services.Interfaces;


namespace SwaggerDiff.Services
{
    public class ClientRequestService : IClientRequestService
    {
        // Only instantiate one static instance of HttpClient
        static readonly HttpClient httpClient = new HttpClient();
        
        // Injected services
        private readonly IUrlService _urlService;
        private readonly ILogger _logger;


        public ClientRequestService(ILogger<IClientRequestService> logger, IUrlService urlService) 
        {
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

        public async void SendSlackMessage(string webServiceName, JObject slackMessage)
        {
            try {
                // Get the webhook URL for posting Slack messages
                Uri requestUrl = new Uri(_urlService.getSlackWebhookUrl());

                // Add config to the request
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUrl);

                // Add slack message as content on the request
                request.Content = new StringContent(slackMessage.ToString(), Encoding.UTF8, "application/json");

                // Invoke HTTP POST request to post Slack message
                HttpResponseMessage response = await httpClient.SendAsync(request);

                // Validate request received a successful status code
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException error)
            {
                _logger.LogInformation($"Failed to post swagger-diff report as Slack message: {error}");
            }
        }
    }
}
