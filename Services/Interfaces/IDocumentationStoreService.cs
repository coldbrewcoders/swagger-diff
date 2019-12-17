
namespace SwaggerDiff.Services.Interfaces
{
    public interface IDocumentationStoreService 
    {
        string this[string webServiceName]
        {
            get;
            set;
        }
    }
}
