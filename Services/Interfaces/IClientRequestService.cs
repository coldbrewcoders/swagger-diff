using System.Net.Http;
using System.Threading.Tasks;

namespace SwaggerDiff.Services.Interfaces
{
    public interface IClientRequestService
    {
        Task<string> FetchServiceSwaggerJsonAsync(string requestUrl);
    }
}
