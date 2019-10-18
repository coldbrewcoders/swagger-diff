using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

      // Register swagger service to run
      services.AddSingleton<ISwaggerService, SwaggerService>(); 
      // TODO: Learn difference between AddSingleton, AddTransient, (one other)
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints => endpoints.MapControllers());

      // Blocking task to get all the swagger JSONs
      app.ApplicationServices.GetService<ISwaggerService>().Initialize();
    }
  }
}
