using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using SwaggerDiff.Services.Interfaces;


namespace SwaggerDiff.Services
{
    public class CompareService : ICompareService
    {
        private readonly ILogger _logger;

        public CompareService(ILogger<CompareService> logger)
        {
            _logger = logger;
        }


        private OpenApiDocument GetDeserializedJsonAsOpenApiDocument(string str)
        {
            // Convert string to byte array
            byte[] byteArray = Encoding.ASCII.GetBytes(str);

            // Convert byte array to stream
            MemoryStream stream = new MemoryStream(byteArray);

            // Return OpenApiDocument object instance for serialized JSON
            return new OpenApiStreamReader().Read(stream, out var diagnostic);
        }

        public async Task CheckServiceForApiChanges(string previousJSON, string freshJSON)
        {
            // Convert serialized JSON swagger definition into instances of OpenApiDocuments
            OpenApiDocument previousApi = GetDeserializedJsonAsOpenApiDocument(previousJSON);
            OpenApiDocument freshApi = GetDeserializedJsonAsOpenApiDocument(freshJSON);

            // Create array of tasks
            Task[] tasks = new Task[] {
                CheckForApiRouteAndHttpMethodAdditions(previousApi, freshApi),
                CheckForApiRouteAndHttpMethodRemovals(previousApi, freshApi)
            };

            // Run all tasks in parallell
            await Task.WhenAll(tasks);
        }



        /*** Async Comparison Methods ***/
        //  TERMS:
        //
        //  'Paths' -> Unique API Routes
        //  'Operations' -> Unique HTTP method for an API Route
        // 

        private async Task CheckForApiRouteAndHttpMethodAdditions(OpenApiDocument previousApi, OpenApiDocument freshApi)
        {
            _logger.LogInformation("Checking for API Route and HTTP Method Additions");

            // Get previous API routes
            OpenApiPaths previousApiRoutes = previousApi.Paths;

            // Get fresh API routes
            OpenApiPaths freshPaths = freshApi.Paths;

            // Iterate over API routes from fresh documentation
            foreach(KeyValuePair<string, OpenApiPathItem> path in freshPaths)
            {
                // Find current API route as string
                string currentApiRoute = path.Key;

                // Check if this API route existed in previous documentation
                if(!previousApiRoutes.ContainsKey(currentApiRoute))
                {
                    // There is a new API route
                    _logger.LogInformation($"New API Route: '{currentApiRoute}'");

                    // Find new API route values
                    OpenApiPathItem newApiRouteValue = path.Value;

                    IDictionary<OperationType, OpenApiOperation> newApiRouteHttpMethodsDict = newApiRouteValue.Operations;

                    // Check if the endpoint item has HTTP methods (endpoint is new so all HTTP methods are new)
                    if(newApiRouteHttpMethodsDict.Count > 0)
                    {
                        // New API route has Operations. Since API route is new, all HTTP methods are also new
                        IList<OperationType> newApiRouteHttpMethodTypes = new List<OperationType>(newApiRouteHttpMethodsDict.Keys);

                        // Iterate through the HTTP methods for the new API route
                        foreach(OperationType newApiRouteHttpMethod in newApiRouteHttpMethodTypes)
                        {
                            // List each HTTP method for new route
                            _logger.LogInformation($"New HTTP Method: '{newApiRouteHttpMethod.ToString()}' for New Route: {currentApiRoute}");
                        }
                    }
                }
                else
                {
                    // Previous API documentation already has this API route, check if there are any new HTTP methods for existing route
                    
                    // Get dictionary of HTTP methods of current API route from fresh documentation
                    OpenApiPathItem freshApiRouteValue = path.Value;
                    IDictionary<OperationType, OpenApiOperation> freshApiRouteHttpMethodsDict = freshApiRouteValue.Operations;

                    // Get dictionary of HTTP methods of current API route from previous documentation
                    OpenApiPathItem previousApiRouteValue = previousApiRoutes[currentApiRoute];
                    IDictionary<OperationType, OpenApiOperation> previousApiRouteHttpMethodsDict = previousApiRouteValue.Operations;
                    
                    // Verify that fresh documentation for this route has HTTP methods
                    if(freshApiRouteHttpMethodsDict.Count > 0)
                    {
                        // Get list of HTTP methods from fresh documentation of this API route
                        IList<OperationType> freshApiRouteHttpMethods = new List<OperationType>(freshApiRouteHttpMethodsDict.Keys);

                        // Get list of HTTP methods from previous documentation of this API route
                        IList<OperationType> previousApiRouteHttpMethods = new List<OperationType>(previousApiRouteHttpMethodsDict.Keys);

                        foreach(OperationType freshHttpMethod in freshApiRouteHttpMethods)
                        {
                            if(!previousApiRouteHttpMethods.Contains(freshHttpMethod))
                            {
                                _logger.LogInformation($"New HTTP Method: '{freshHttpMethod.ToString()}' for Existing Route: '{currentApiRoute}'");
                            }
                        }
                    }

                }
            }

            await Task.Delay(1); 
        }

        private async Task CheckForApiRouteAndHttpMethodRemovals(OpenApiDocument previousApi, OpenApiDocument freshApi)
        {
            _logger.LogInformation("Checking for API Route and HTTP Method Removals");

            // Get previous API routes
            OpenApiPaths previousPaths = previousApi.Paths;

            // Get fresh API routes
            OpenApiPaths freshPaths = freshApi.Paths;

            await Task.Delay(1); 
        }
    }
}