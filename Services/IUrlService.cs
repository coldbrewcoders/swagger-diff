using System.Collections.Generic;


namespace SwaggerDiff.Services
{
    public interface IUrlService
    {
        // Properties
        HashSet<string> ServiceNames { get; }
        string SlackWebhookUrl { get; }

        // Methods
        bool IsValidServiceName(string serviceName);
        string GetSwaggerDocumentUrl(string serviceName);
    }
}