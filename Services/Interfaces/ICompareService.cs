using System.Threading.Tasks;


namespace SwaggerDiff.Services.Interfaces
{
    public interface ICompareService
    {
        Task CheckServiceForApiChanges(string previousJSON, string freshJSON);
    }
}