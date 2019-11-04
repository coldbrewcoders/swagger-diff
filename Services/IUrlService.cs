namespace SwaggerDiff.Services
{
    public interface IUrlService
    {
        string[] GetServiceNames();
        bool IsValidServiceName(string serviceName);
        string GetSwaggerDocumentUrl(string serviceName);
        string GetSlackWebhookUrl();
    }
}