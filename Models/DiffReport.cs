using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;


namespace SwaggerDiff.Models
{
    public class DiffReport
    {
        // Name of web-service associated with diff-report 
        public string WebServiceName;

        // New Routes and all HTTP methods belonging to new route
        private Dictionary<string, List<OperationType>> NewRoutesAndHttpMethods = new Dictionary<string, List<OperationType>>();

        // Removed Routes and all HTTP methods removed from route
        private Dictionary<string, List<OperationType>> RemovedRoutesAndHttpMethods = new Dictionary<string, List<OperationType>>();


        public DiffReport(string webServiceName)
        {
            WebServiceName = webServiceName;
        }


        public void RecordAddedRouteInformation(string route, OperationType httpMethod)
        {
            // Check if route already exist as key..
            if(!NewRoutesAndHttpMethods.ContainsKey(route)) {
                // On the first occurrence of a route, init new List<OperationType> for Value of NewRoutesAndHttpMethods
                NewRoutesAndHttpMethods[route] = new List<OperationType>();
            }

            // Add new HTTP method to added route
            NewRoutesAndHttpMethods[route].Add(httpMethod);
        }

        public void RecordRemovedRouteInformation(string route, OperationType httpMethod)
        {
            // Check if route already exist as key..
            if(!RemovedRoutesAndHttpMethods.ContainsKey(route)) {
                // On the first occurrence of a route, init new List<OperationType> for Value of RemovedRoutesAndHttpMethods
                RemovedRoutesAndHttpMethods[route] = new List<OperationType>();
            }

            // Add http method to removed route
            RemovedRoutesAndHttpMethods[route].Add(httpMethod);
        }

        public JObject GenerateSlackMessageContent()
        {
            // Use LINQ to create a list of { title, value } anonymous types
            var endpointsAdded = NewRoutesAndHttpMethods.SelectMany(x => x.Value, (route, httpMethod) => new {
                title = $"{HttpMethodsEnumToString(httpMethod)}: {route.Key}",
                value = "Added",
                @short = false
            }).ToList();

            // Use LINQ to create a list of { title, value } anonymous types
            var endpointsRemoved = RemovedRoutesAndHttpMethods.SelectMany(x => x.Value, (route, httpMethod) => new {
                title = $"{HttpMethodsEnumToString(httpMethod)}: {route.Key}",
                value = "Removed",
                @short = false
            }).ToList();

            // Combine lists Of added and removed endpoints
            var fieldsList = endpointsAdded.Concat(endpointsRemoved);
            
            // Create Json Object to represent slack message
            JObject slackMessage = JObject.FromObject(new {
                attachments = new[] {
                    new {
                        title = $"API Change Detected: {WebServiceName}",
                        color = $"{((NumberOfRoutesRemoved() > 0) ? "danger" : "good")}",
                        fields = fieldsList,
                        footer = "Swagger Diff",
                        ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                    }
                }
            });

            return slackMessage;
        }
        

        private string HttpMethodsEnumToString(OperationType httpMethod)
        {
            // Convert OpenApi.Models.OperationType enum to uppercase string
            return httpMethod.ToString().ToUpper();
        }

        public int NumberOfRouteChanges()
        {
            // Total number of route changes (Added and Removed)
            return NewRoutesAndHttpMethods.Keys.Count + RemovedRoutesAndHttpMethods.Keys.Count;
        }

        private int NumberOfRoutesAdded()
        {
            // Number of routes added 
            return NewRoutesAndHttpMethods.Keys.Count;
        }

        private int NumberOfRoutesRemoved()
        {   
            // Number of routes removed
            return RemovedRoutesAndHttpMethods.Keys.Count;
        }
    }
}

