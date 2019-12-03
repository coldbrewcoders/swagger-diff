
namespace SwaggerDiff.Services.Interfaces
{
    public interface IDocumentStoreService 
    {
        string GetValue(string webServiceName);
        void SetValue(string webServiceName, string documentationJson);
    }
}
