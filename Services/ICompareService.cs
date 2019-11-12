namespace SwaggerDiff.Services
{
    public interface ICompareService
    {
        bool AreJSONDocumentsIdentical(string previousJSON, string freshJSON);
        void CheckServiceForApiChanges(string previousJSON, string freshJSON);
    }
}