using System.Net.Http;
using System.Threading.Tasks;

namespace SwaggerDiff.Services
{
    public class ClientRequestManager
    {
        // Only instantiate one instance of HttpClient
        static readonly HttpClient httpClient = new HttpClient();

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
            catch(HttpRequestException error)
            {
                throw error;
            }
        }

    }
}
