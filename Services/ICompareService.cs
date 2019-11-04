namespace SwaggerDiff.Services
{
    public interface ICompareService
    {
        bool AreJSONDocumentsIdentical(string previousJSON, string freshJSON);
        void CompareServiceApiSpecs(string previousJSON, string freshJSON);
    }
}