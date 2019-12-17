namespace SwaggerDiff.Services.Interfaces
{
    public interface IUrlService
    {
        string GetSwaggerDocumentUrl(string serviceName);
        string getSlackWebhookUrl();
    }
}