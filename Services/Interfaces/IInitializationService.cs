using System.Threading.Tasks;

namespace SwaggerDiff.Services.Interfaces
{
    public interface IInitializationService
    {
        void Initialize();
        bool IsValidWebServiceName(string serviceName);
        Task<bool> ReattemptDocumentFetch(string webServiceName);
    }
}