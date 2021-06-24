using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using surtus_api_restful.Middlewares.ApiWrapper.Wrappers;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace surtus_api_restful.Middlewares.ApiWrapper
{
    public class ApiWrapperMiddleware
    {
        public const string RESPONSE_HEADER_REQUEST_TIME = "X-Request-Time";
        public const string RESPONSE_HEADER_RESPONSE_TIME = "X-Response-Time";
        private readonly RequestDelegate _next;

        public ApiWrapperMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(RESPONSE_HEADER_REQUEST_TIME))
                {
                    context.Response.Headers.Add(ApiWrapperMiddleware.RESPONSE_HEADER_REQUEST_TIME, now.ToString());
                }
                context.Response.Headers.Add(ApiWrapperMiddleware.RESPONSE_HEADER_RESPONSE_TIME, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
                return Task.CompletedTask;
            });

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = HandleException(ex);

                var serializerSettings = (context.RequestServices.GetRequiredService<IOptions<JsonOptions>>()).Value.JsonSerializerOptions;

                context.Response.ContentType = "application/json";
                if (context.Response.StatusCode != response.StatusCode)
                {
                    context.Response.StatusCode = response.StatusCode;
                }

                await JsonSerializer.SerializeAsync(context.Response.Body, response, response.GetType(), serializerSettings);

                await context.Response.Body.FlushAsync();
            }
        }

        private static ApiResponse<ApiError> HandleException(Exception exception)
        {
            ApiError apiError = null;
            int code = 0;

            if (exception is ApiException)
            {
                var ex = exception as ApiException;
                apiError = new ApiError(ex.Message)
                {
                    ErrorCode = ex.ErrorCode,
                    ValidationErrors = ex.Errors
                };
                code = ex.StatusCode;
            }
            else if (exception is UnauthorizedAccessException)
            {
                apiError = new ApiError("unauthorized.");
                code = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                #if !DEBUG
                var msg = "An unhandled error occurred.";
                string stack = null;
                #else
                var msg = exception.GetBaseException().Message;
                string stack = exception.StackTrace;
                #endif
                apiError = new ApiError(msg)
                {
                    Details = stack
                };
                code = (int)HttpStatusCode.InternalServerError;
            }

            return new ApiResponse<ApiError>(code, "error", apiError);
        }
    }

}
