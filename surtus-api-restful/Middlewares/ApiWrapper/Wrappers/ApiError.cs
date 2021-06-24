using System.Collections.Generic;

namespace surtus_api_restful.Middlewares.ApiWrapper.Wrappers
{
    public class ApiError
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public IDictionary<string, Dictionary<string, Dictionary<string, object>>> ValidationErrors { get; set; }

        public ApiError(string message)
        {
            ErrorCode = -1;
            Message = message;
        }
    }

}
