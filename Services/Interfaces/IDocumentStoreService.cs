using System.Collections.Generic;


namespace SwaggerDiff.Services.Interfaces
{
    public interface IDocumentStoreService 
    {
        string GetValue(string webServiceName);
        void SetValue(string webServiceName, string documentationJson);
        List<KeyValuePair<string, string>> GetDocumentStoreContents();
    }
}
