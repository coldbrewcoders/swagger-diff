namespace SwaggerDiff.Services
{
    public interface IUrlService
    {
        string[] GetServiceNames();
        bool IsValidServiceName(string serviceName);
        string GetUrl(string serviceName);
    }
}