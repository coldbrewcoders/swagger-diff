using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SwaggerDiff
{
  public class Program
  {
    public static void Main(string[] args)
    {
      // Create and run Web Service
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
      .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        }
      )
      .ConfigureWebHostDefaults(webBuilder =>
        {
          webBuilder.UseStartup<Startup>();
        }
      );
  }
}
