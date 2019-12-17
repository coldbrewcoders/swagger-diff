using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System.Collections.Generic;

// Models and Service Interfaces
using SwaggerDiff.Models;
using SwaggerDiff.Services.Interfaces;


/*******************************************
*
* Terms:
*   'Route' -> /url/to/endpoint/{routeParam}
*   'HTTP Method': GET, PUT, POST, etc...
*   'API Endpoint' -> Combination of an HTTP method and route (<HTTP METHOD> </ROUTE>)
*       Ex. GET /users/info/{userId}
*
*******************************************/


namespace SwaggerDiff.Services
{
    public class CompareService : ICompareService
    {
        private readonly IClientRequestService _clientRequestService;


        // Constructor
        public CompareService(IClientRequestService clientRequestService) 
        {
            _clientRequestService = clientRequestService;
        }


        // Public methods
        public async Task CheckServiceForApiChanges(string webServiceName, string previousApiDocumentJSON, string freshApiDocumentJSON)
        {
            // Convert serialized JSON swagger definition into instances of OpenApiDocuments
            OpenApiDocument previousApiDocument = GetDeserializedJsonAsOpenApiDocument(previousApiDocumentJSON);
            OpenApiDocument freshApiDocument = GetDeserializedJsonAsOpenApiDocument(freshApiDocumentJSON);

            // Create instance of DiffReport class to store API changes
            DiffReport diffReport = new DiffReport(webServiceName);

            // Create array of tasks
            Task[] tasks = new Task[] {
                Task.Run(() => CheckForApiRouteAndHttpMethodAdditions(previousApiDocument, freshApiDocument, diffReport)),
                Task.Run(() => CheckForApiRouteAndHttpMethodRemovals(previousApiDocument, freshApiDocument, diffReport))
            };

            // Run all tasks in parallell
            await Task.WhenAll(tasks);

            // Get slack message JSON from diff report
            JObject slackMessage = diffReport.GenerateSlackMessageContent();

            // Make Client request to post slack message
            _clientRequestService.SendSlackMessage(diffReport.WebServiceName, slackMessage);
        }


        // Private methods
        private OpenApiDocument GetDeserializedJsonAsOpenApiDocument(string str)
        {
            // Convert string to byte array
            byte[] byteArray = Encoding.ASCII.GetBytes(str);

            // Convert byte array to stream
            MemoryStream stream = new MemoryStream(byteArray);

            // Return OpenApiDocument object instance for serialized JSON
            return new OpenApiStreamReader().Read(stream, out var diagnostic);
        }


        /*** API document comparison checks ***/

        private void CheckForApiRouteAndHttpMethodAdditions(OpenApiDocument previousApiDocument, OpenApiDocument freshApiDocument, DiffReport diffReport)
        {
            // Get information for all routes in previous API documentation
            OpenApiPaths previousApiDocumentRoutes = previousApiDocument.Paths;

            // Get information for all routes in fresh API documentation
            OpenApiPaths freshApiDocumentRoutes = freshApiDocument.Paths;

            // Iterate over all routes in freshly downloaded API documentation
            foreach(KeyValuePair<string, OpenApiPathItem> route in freshApiDocumentRoutes)
            {
                // Find current API route as string
                string currentApiRoute = route.Key;

                // Check if this route in the fresh API documentation in a new addition
                if(!previousApiDocumentRoutes.ContainsKey(currentApiRoute))
                {
                    // NOTE: New route detected, therefore all HTTP methods associated with this route are new

                    // Find all HTTP methods associated with this route (HTTP methods represented as enum type OpenApi.Models.OperationType)
                    OpenApiPathItem newApiRouteValue = route.Value;
                    IDictionary<OperationType, OpenApiOperation> newApiRouteHttpMethodsDict = newApiRouteValue.Operations;

                    // Verify route has associated HTTP methods
                    if(newApiRouteHttpMethodsDict.Count > 0)
                    {
                        // Get list of all HTTP methods associated with this new route
                        IList<OperationType> newApiRouteHttpMethodTypes = new List<OperationType>(newApiRouteHttpMethodsDict.Keys);

                        // Iterate through the new route's HTTP methods
                        foreach(OperationType newApiRouteHttpMethod in newApiRouteHttpMethodTypes)
                        {
                            // Save added route/HTTP method information for API diff report
                            diffReport.RecordAddedRouteInformation(currentApiRoute, newApiRouteHttpMethod);
                        }
                    }
                }
                else
                {
                    // Previous API documentation already has this API route, check if there are any new HTTP methods for the existing route
                    
                    // Get dictionary of HTTP methods for current route from fresh API documentation
                    OpenApiPathItem freshApiDocumentRouteValue = route.Value;
                    IDictionary<OperationType, OpenApiOperation> freshApiDocumentRouteInfo = freshApiDocumentRouteValue.Operations;

                    // Get dictionary of HTTP methods for current route from previous API documentation
                    OpenApiPathItem previousApiDocumentRouteValue = previousApiDocumentRoutes[currentApiRoute];
                    IDictionary<OperationType, OpenApiOperation> previousApiDocumentRouteInfo = previousApiDocumentRouteValue.Operations;
                    
                    // Verify that fresh API documentation for this route contains HTTP methods
                    if(freshApiDocumentRouteInfo.Count > 0)
                    {
                        // For this route, get list of HTTP methods in fresh API documentation
                        IList<OperationType> freshApiRouteHttpMethods = new List<OperationType>(freshApiDocumentRouteInfo.Keys);

                        // For this route, get list of HTTP methods in previous API documentation
                        IList<OperationType> previousApiRouteHttpMethods = new List<OperationType>(previousApiDocumentRouteInfo.Keys);

                        // Iterate over all HTTP methods listed in fresh API documentation for this route
                        foreach(OperationType freshHttpMethod in freshApiRouteHttpMethods)
                        {
                            // For this route, if an HTTP method exists in fresh API documentation but not previous API documentation,
                            // We have found a new API endpoint
                            if(!previousApiRouteHttpMethods.Contains(freshHttpMethod))
                            {
                                // Record newly detected API endpoint in diff report object
                                diffReport.RecordAddedRouteInformation(currentApiRoute, freshHttpMethod);
                            }
                        }
                    }
                }
            }
        }

        private void CheckForApiRouteAndHttpMethodRemovals(OpenApiDocument previousApiDocument, OpenApiDocument freshApiDocument, DiffReport diffReport)
        {
            // Get information for all routes in previous API documentation
            OpenApiPaths previousApiDocumentRoutes = previousApiDocument.Paths;

            // Get information for all routes in fresh API documentation
            OpenApiPaths freshApiDocumentRoutes = freshApiDocument.Paths;

            // Iterate over all routes in previously stored API documentation
            foreach(KeyValuePair<string, OpenApiPathItem> route in previousApiDocumentRoutes)
            {
                // Find current API route as string
                string currentApiRoute = route.Key;

                // Check if this route in the previously downloaded API documentation has been removed
                if(!freshApiDocumentRoutes.ContainsKey(currentApiRoute))
                {
                    // NOTE: This route has been removed, therefore all HTTP methods associated with this route have been removed

                    // Find all HTTP methods associated with this route (HTTP methods represented as enum type OpenApi.Models.OperationType)
                    OpenApiPathItem removedApiRouteValue = route.Value;
                    IDictionary<OperationType, OpenApiOperation> removedApiRouteHttpMethodsDict = removedApiRouteValue.Operations;

                    // Verify route has associated HTTP methods
                    if(removedApiRouteHttpMethodsDict.Count > 0)
                    {
                        // Get list of all HTTP methods associated with this removed route
                        IList<OperationType> removedApiRouteHttpMethodTypes = new List<OperationType>(removedApiRouteHttpMethodsDict.Keys);

                        // Iterate through the removed route's HTTP methods
                        foreach(OperationType removedApiRouteHttpMethod in removedApiRouteHttpMethodTypes)
                        {
                            // Save removed route/HTTP method information for API diff report
                            diffReport.RecordRemovedRouteInformation(currentApiRoute, removedApiRouteHttpMethod);
                        }
                    }
                }
                else
                {
                    // New API documentation has this route, check if any HTTP methods have been removed

                    // Get dictionary of HTTP methods for current route from previous API documentation
                    OpenApiPathItem previousApiDocumentRouteValue = route.Value;
                    IDictionary<OperationType, OpenApiOperation> previousApiDocumentRouteInfo = previousApiDocumentRouteValue.Operations;

                    // Get dictionary of HTTP methods for current API route from fresh API documentation
                    OpenApiPathItem freshApiRouteValue = freshApiDocumentRoutes[currentApiRoute];
                    IDictionary<OperationType, OpenApiOperation> freshApiDocumentRouteInfo = freshApiRouteValue.Operations;

                    // Verify that previous API documentation for this route contains HTTP methods
                    if(previousApiDocumentRouteInfo.Count > 0)
                    {
                        // For this route, get list of HTTP methods in previous API documentation
                        IList<OperationType> previousApiRouteHttpMethods = new List<OperationType>(previousApiDocumentRouteInfo.Keys);

                        // For this route, get list of HTTP methods in fresh API documentation
                        IList<OperationType> freshApiRouteHttpMethods = new List<OperationType>(freshApiDocumentRouteInfo.Keys);

                        // Iterate over all HTTP methods listted in previous API documentation for this route 
                        foreach(OperationType previousHttpMethod in previousApiRouteHttpMethods)
                        {
                            // For this route, if an HTTP method exists in previous API documentation but not fresh API documentation,
                            // We have detected the removal of an API endpoint
                            if(!freshApiRouteHttpMethods.Contains(previousHttpMethod))
                            {
                                // Record removal of API endpoint in diff report object
                                diffReport.RecordRemovedRouteInformation(currentApiRoute, previousHttpMethod);
                            }
                        }
                    }
                }
            }
        }


        // TODO: Add additional checks for endpoints that exist in previous and fresh documentation
        // * On PUT, POST, PATCH and DELETE, check for request payload format changes
        // * On GET, POST, PUT, PATCH and DELETE, check for response payload format changes
        // * On all HTTP methods, check for query string changes
    }
}