using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SwaggerDiff.Models;
using SwaggerDiff.Services;


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
            // Add in-memory DB context
            services.AddDbContext<SwaggerDiffContext>(opt => opt.UseInMemoryDatabase("SwaggerDiffDB"), ServiceLifetime.Singleton);

            // Add controllers
            services.AddControllers();

            // Inject url service
            services.AddSingleton<IUrlService, UrlService>();
            
            // Inject client request service
            services.AddSingleton<IClientRequestService, ClientRequestService>();

            // Inject initialization service
            services.AddSingleton<IInitializationService, InitializationService>();

            // Inject compare service
            services.AddSingleton<ICompareService, CompareService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            // Blocking task to load all initial swagger documentation for each web service (blocking)
            app.ApplicationServices.GetService<IInitializationService>().Initialize().GetAwaiter().GetResult();

            // Apply routing
            app.UseRouting();

            // Enable cors to allow exposed webhooks to be called from any origin
            app.UseCors();

            // Apply Controllers as route handlers 
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
  }
}
