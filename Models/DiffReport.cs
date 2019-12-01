using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;


namespace SwaggerDiff.Models
{
    public class DiffReport
    {
        // New Routes and all HTTP methods belonging to new route
        private Dictionary<string, List<OperationType>> NewRoutesAndHttpMethods = new Dictionary<string, List<OperationType>>();

        // Removed Routes and all HTTP methods removed from route
        private Dictionary<string, List<OperationType>> RemovedRoutesAndHttpMethods = new Dictionary<string, List<OperationType>>();


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

        // TODO: Remove method
        public void LogDiffReport()
        {
            Console.WriteLine("\n\n\n *** Diff Report *** \n");

            // If no endpoint changes detected...
            if(NumberOfRouteChanges() == 0) {
                Console.WriteLine("No API Changes Detected.\n\n");
                return;
            }

            // If endpoint addition(s) detected..
            if(NumberOfRoutesAdded() > 0)
            {
                Console.WriteLine("Added Routes:");

                foreach(string route in NewRoutesAndHttpMethods.Keys)
                {
                    foreach(OperationType httpMethod in NewRoutesAndHttpMethods[route])
                    {
                        Console.WriteLine($"\t Added {HttpMethodsEnumToString(httpMethod)}: {route}");
                    }
                }

                Console.WriteLine("\n\n");
            }

            // If endpoint removal(s) detected...
            if(NumberOfRoutesRemoved() > 0)
            {
                Console.WriteLine("Removed Routes:");

                foreach(string route in RemovedRoutesAndHttpMethods.Keys)
                {
                    foreach(OperationType httpMethod in RemovedRoutesAndHttpMethods[route])
                    {
                        Console.WriteLine($"\t Removed {HttpMethodsEnumToString(httpMethod)}: {route}");
                    }
                }

                Console.WriteLine("\n\n");
            }
        }
        

        private string HttpMethodsEnumToString(OperationType httpMethod)
        {
            return httpMethod.ToString().ToUpper();
        }

        private int NumberOfRouteChanges()
        {
            return NewRoutesAndHttpMethods.Keys.Count + RemovedRoutesAndHttpMethods.Keys.Count;
        }

        private int NumberOfRoutesAdded()
        {
            return NewRoutesAndHttpMethods.Keys.Count;
        }

        private int NumberOfRoutesRemoved()
        {
            return RemovedRoutesAndHttpMethods.Keys.Count;
        }
    }
}

