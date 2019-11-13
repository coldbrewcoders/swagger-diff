using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;


namespace SwaggerDiff.Services
{
    public class CompareService : ICompareService
    {
        private string GetJsonMD5Hash(MD5 md5Hash, string str)
        {   
            // Get the MD5 hash of serialized JSON document as a byte array
            byte[] byteArray = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(str));

            // Convert byte array to string and return value
            return Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
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

        public bool AreJSONDocumentsIdentical(string previousJSON, string freshJSON)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                // Generate MD5 hashes of two passed serialized JSON documents
                string previousJsonHash = this.GetJsonMD5Hash(md5Hash, previousJSON);
                string freshJSONHash = this.GetJsonMD5Hash(md5Hash, freshJSON);

                // Compare hash values. If identical, we need no additional checks because document JSON document has not changed
                if (previousJsonHash == freshJSONHash) 
                {
                    return true;
                }
                
                return false;
            }
        }

        public void CheckServiceForApiChanges(string previousJSON, string freshJSON)
        {
            // Convert serialized JSON swagger definition into instances of OpenApiDocuments
            OpenApiDocument previousApi = GetDeserializedJsonAsOpenApiDocument(previousJSON);
            OpenApiDocument freshApi = GetDeserializedJsonAsOpenApiDocument(freshJSON);

            // Create array of tasks
            Task[] tasks = new Task[] {
                CheckForEndpointAddition(previousApi, freshApi),
                CheckForEndpointRemoval(previousApi, freshApi)
            };

            // Run all tasks in parallell
            Task.WaitAll(tasks);
        }

        private async Task CheckForEndpointAddition(OpenApiDocument previousApi, OpenApiDocument freshApi)
        {
                
            Console.WriteLine("Checking for endpoint Addition");

            // Get previous API routes
            OpenApiPaths previousPaths = previousApi.Paths;

            // Get fresh API routes
            OpenApiPaths freshPaths = freshApi.Paths;

            Console.WriteLine(freshPaths);

            await Task.Delay(1000); 
        }

        private async Task CheckForEndpointRemoval(OpenApiDocument previousApi, OpenApiDocument freshApi)
        {
            Console.WriteLine("Checking for endpoint Removal");

            // Get previous API routes
            OpenApiPaths previousPaths = previousApi.Paths;

            // Get fresh API routes
            OpenApiPaths freshPaths = freshApi.Paths;

            Console.WriteLine(freshPaths);

            await Task.Delay(1000); 
        }
    }
}