using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SwaggerDiff.Services;
using SwaggerDiff.Services.Interfaces;


namespace SwaggerDiff
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to inject services to the app.
        public void ConfigureServices(IServiceCollection services)
        {
            // Inject service to store Swagger documentation JSON values in thread-safe way
            services.AddSingleton<IDocumentStoreService, DocumentStoreService>();

            // Inject service to manage URLs for client requests
            services.AddSingleton<IUrlService, UrlService>();
            
            // Inject service for making client requests
            services.AddSingleton<IClientRequestService, ClientRequestService>();

            // Inject service for performing Swagger documentation file comparisons
            services.AddSingleton<ICompareService, CompareService>();

            // Inject initialization service
            services.AddSingleton<IInitializationService, InitializationService>();

            // Add controllers
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            // Blocking task to load all initial swagger documentation for each web service (blocking)
            app.ApplicationServices.GetService<IInitializationService>().Initialize();

            // Apply routing
            app.UseRouting();

            // Enable cors to allow exposed webhooks to be called from any origin
            app.UseCors();

            // Apply Controllers as route handlers 
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
  }
}
