using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using SwaggerDiff.Models;
using SwaggerDiff.Services;

namespace SwaggerDiff
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add in-memory DB context
            services.AddDbContext<SwaggerDiffContext>(opt => opt.UseInMemoryDatabase("SwaggerDiffDB"), ServiceLifetime.Singleton);

            // Add SwaggerDiff controllers
            services.AddControllers();

            // Register swagger service URL manager
            services.AddSingleton<IUrlService, UrlService>();
            
            // Register client request manager
            services.AddSingleton<IClientRequestService, ClientRequestService>();

            // Register swagger service to run
            services.AddSingleton<IInitializationService, InitializationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            // Blocking task to get all the swagger JSON documents from each service
            app.ApplicationServices.GetService<IInitializationService>().Initialize();

            if (env.IsDevelopment())
            {
                logger.LogInformation("Running in development mode.");
            }

            if(env.IsProduction()) 
            {
                logger.LogInformation("Running in development production mode.");
            }

            // Apply routing
            app.UseRouting();

            // Enable cors since we are exposing webhooks (Allow requests from any origin)
            app.UseCors();

            // Apply Controllers as route handlers
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
  }
}
