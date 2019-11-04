using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SwaggerDiff.Services
{
    public class ClientRequestService : IClientRequestService
    {
        // Only instantiate one instance of HttpClient
        static readonly HttpClient httpClient = new HttpClient();

        // Slack webhook URL to post slack messages to
        private readonly string _slackWebhookUrl;
        
        public ClientRequestService() {
            // Init slack webhook url
            _slackWebhookUrl = Environment.GetEnvironmentVariable("SWAGGER_DIFF_SLACK_WEBHOOK");
        }

        public async Task<string> FetchServiceSwaggerJsonAsync(string requestUrl)
        {
            try 
            {
                // Invoke a get request to fetch swagger JSON document async
                HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

                // Ensure response code is a success type
                response.EnsureSuccessStatusCode();

                // Get the response body
                string responseBody = await response.Content.ReadAsStringAsync();

                // Return response body as string
                return responseBody;
            }
            catch (HttpRequestException error)
            {
                throw error;
            }
        }

        // TODO: Write method that sends client request to slack webhook (results in slack notification)

    }
}
