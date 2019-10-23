namespace SwaggerDiff.Services
{
    public interface ISwaggerServiceUrlManager
    {
        string[] ServiceNames { get; }
        string GetUrl(string serviceName);
    }
}