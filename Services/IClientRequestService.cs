using System.Net.Http;
using System.Threading.Tasks;

namespace SwaggerDiff.Services
{
    public interface IClientRequestService
    {
        Task<string> FetchServiceSwaggerJsonAsync(string requestUrl);
    }
}
