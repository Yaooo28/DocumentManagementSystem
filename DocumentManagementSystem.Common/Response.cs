namespace DocumentManagementSystem.Common
{
    public class Response : IResponse
    {
        public Response(ResponseType responseType)
        {
            ResponseType = responseType;
        }

        public Response(ResponseType responseType, string message)
        {
            Message = message;
            ResponseType = responseType;
        }

        public string Message { get; set; }

        public ResponseType ResponseType { get; set; }
    }

    public enum ResponseType
    {
        Success,
        ValidationError,
        NotFound
    }
}
