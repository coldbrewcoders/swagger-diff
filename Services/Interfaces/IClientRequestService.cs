using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SwaggerDiff.Services.Interfaces
{
    public interface IClientRequestService
    {
        Task<string> FetchServiceSwaggerJsonAsync(string webServiceName);
        void SendSlackMessage(string webServiceName, JObject slackMessage);
    }
}
