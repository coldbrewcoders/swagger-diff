using System.Net.Http;
using System.Threading.Tasks;

namespace SwaggerDiff.Services
{
    public interface IClientRequestManager
    {
        Task<string> FetchServiceSwaggerJsonAsync(string requestUrl);
    }
}
