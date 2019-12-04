
namespace SwaggerDiff.Services.Interfaces
{
    public interface IDocumentationStoreService 
    {
        string GetValue(string webServiceName);
        void SetValue(string webServiceName, string documentationJson);
    }
}
