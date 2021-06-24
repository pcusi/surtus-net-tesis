namespace surtus_api_restful.Middlewares.ApiWrapper.Wrappers
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }

        public ApiResponse(int statusCode, string message = "", T result = default(T))
        {
            StatusCode = statusCode;
            Message = message;
            Result = result;
        }
    }

}
