using System;
using System.Collections.Generic;

namespace surtus_api_restful.Middlewares.ApiWrapper.Wrappers
{
    public class ApiException : Exception
    {
        public int ErrorCode { get; set; }
        public int StatusCode { get; set; }
        public IDictionary<string, Dictionary<string, Dictionary<string, object>>> Errors { get; set; }

        public ApiException(string message, int statusCode = 500, int errorCode = -1, IDictionary<string, Dictionary<string, Dictionary<string, object>>> errors = null)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
            Errors = errors;
        }

        public ApiException(Exception ex, int statusCode = 500)
            : base(ex.Message)
        {
            StatusCode = statusCode;
        }
    }

}
