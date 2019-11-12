
namespace SwaggerDiff.Models
{
    class ErrorObject
    {
        public readonly string ErrorMessage;
        public readonly string ErrorCode;
        
        public ErrorObject(string errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}