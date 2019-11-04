using System.Text;
using System.Security.Cryptography;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System.IO;


namespace SwaggerDiff.Services
{
    public class CompareService : ICompareService
    {
        private string GetJsonMD5Hash(MD5 md5Hash, string str)
        {   
            // Get the MD5 hash of JSON document as a byte array
            byte[] byteArray = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(str));

            // User string builder to return a hex string as hash
            StringBuilder sBuilder = new StringBuilder();

            // Convert byte array back into string
            for (int i = 0; i < byteArray.Length; i++)
            {
                sBuilder.Append(byteArray[i].ToString("x2"));
            }

            // Return MD5 hash of string
            return sBuilder.ToString();
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

        public void CompareServiceApiSpecs(string previousJSON, string freshJSON)
        {
            // Convert serialized JSON swagger definition into instances of OpenApiDocuments
            OpenApiDocument previousApiSpecs = this.GetDeserializedJsonAsOpenApiDocument(previousJSON);
            OpenApiDocument freshApiSpecs = this.GetDeserializedJsonAsOpenApiDocument(freshJSON);

            // TODO: Use two OpenApiDocument instaces to find diffs

            // TODO: Send slack notification describing diff
        }
    }
}