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
            // Check that all required env variables exist
            ValidateRequiredEnvVariables();

            // Create and run Web Service
            CreateHostBuilder(args).Build().Run();
        }

        private static void ValidateRequiredEnvVariables()
        {
            // Get required env variables for application to run
            string hostname = Environment.GetEnvironmentVariable("SWAGGER_DIFF_HOSTNAME");
            string apiVersion = Environment.GetEnvironmentVariable("SWAGGER_DIFF_API_VERSION");
            string serviceNames = Environment.GetEnvironmentVariable("SWAGGER_DIFF_SERVICENAMES");
            string slackWebhookUrl = Environment.GetEnvironmentVariable("SWAGGER_DIFF_SLACK_WEBHOOK");

            bool isMissingRequiredEnvironmentVariable = true;

            if (hostname == null)
            {
                Console.WriteLine("Missing required environment variable 'SWAGGER_DIFF_HOSTNAME'");
            }
            else if (apiVersion == null)
            {
                Console.WriteLine("Missing required environment variable 'SWAGGER_DIFF_API_VERSION'");
            }
            else if (serviceNames == null)
            {
                Console.WriteLine("Missing required environment variable 'SWAGGER_DIFF_SERVICENAMES'");
            }
            else if (slackWebhookUrl == null)
            {
                Console.WriteLine("Missing required environment variable 'SWAGGER_DIFF_SLACK_WEBHOOK'");
            }
            else 
            {
                isMissingRequiredEnvironmentVariable = false;
            }

            if (isMissingRequiredEnvironmentVariable) {
                // Close application
                Environment.Exit(1);
            }
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
