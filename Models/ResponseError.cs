namespace SwaggerDiff.Models
{
    class ErrorObject
    {
        private readonly string ErrorMessage;
        private readonly string ErrorCode;
        
        public ErrorObject(string errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}