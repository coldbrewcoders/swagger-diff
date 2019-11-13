using System.Threading.Tasks;


namespace SwaggerDiff.Services
{
    public interface ICompareService
    {
        bool AreJSONDocumentsIdentical(string previousJSON, string freshJSON);
        Task CheckServiceForApiChanges(string previousJSON, string freshJSON);
    }
}