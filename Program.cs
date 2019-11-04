using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace SwaggerDiff
{
  public class Program
  {
    public static void Main(string[] args)
    {
      // Get required env variables for application to run
      string hostname = Environment.GetEnvironmentVariable("SWAGGER_DIFF_HOSTNAME");
      string apiVersion = Environment.GetEnvironmentVariable("SWAGGER_DIFF_API_VERSION");
      string serviceNames = Environment.GetEnvironmentVariable("SWAGGER_DIFF_SERVICENAMES");
      string slackWebhookUrl = Environment.GetEnvironmentVariable("SWAGGER_DIFF_SLACK_WEBHOOK");

      // Check if any required env variables are missing
      if (hostname == null || apiVersion == null || serviceNames == null || slackWebhookUrl == null) 
      {
          Console.WriteLine("Missing required environment variables. Please provide all required (SWAGGER_DIFF_HOSTNAME, SWAGGER_DIFF_API_VERSION and SWAGGER_DIFF_SERVICENAMES, SWAGGER_DIFF_SLACK_WEBHOOK).");

          // Close application
          Environment.Exit(1);
      }

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
