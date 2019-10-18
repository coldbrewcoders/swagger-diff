using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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

      // Register swagger service
      services.AddSingleton<ISwaggerService, SwaggerService>();
      // TODO: Learn difference between AddSingleton, AddTransient, (one other)

      // Registering the swagger services with http client (DI)
      services.AddHttpClient();

      // services.AddHttpClient(<ISwaggerService, SwaggerService); TODO: Why doesn't this work but the above does?
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      // Captures exception instances from the pipeline and generates HTML error responses
      if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

      // Middleware for redirecting HTTP Requests to HTTPS
      app.UseHttpsRedirection();

      app.UseRouting();

      // Enable authorization capabilities
      app.UseAuthorization();

      app.UseEndpoints(endpoints => endpoints.MapControllers());

      // Fire off async task to get all the swagger JSONs
      Task initializeSwaggerTask = Task.Run(() => app.ApplicationServices.GetService<ISwaggerService>().Initialize());

      // Wait until initialize swagger task is complete
      initializeSwaggerTask.Wait();
    }
  }
}
