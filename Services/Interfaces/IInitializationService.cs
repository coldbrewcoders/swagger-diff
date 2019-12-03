using System.Threading.Tasks;

namespace SwaggerDiff.Services.Interfaces
{
    public interface IInitializationService
    {
        void Initialize();
        bool IsValidWebServiceName(string serviceName);
        Task ReattemptDocumentFetch(string webServiceName);
    }
}