using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using surtus_api_restful.Middlewares.ApiWrapper;
using surtus_api_restful.Middlewares.ApiWrapper.Wrappers;
using surtus_api_restful.Models;
using surtus_api_restful.Resources;

namespace surtus_api_restful.Controllers
{
    [Route("api/[controller]")]
    public class AppController : ControllerBase
    {
        protected readonly SurtusDbContext _db;
        protected readonly IConfiguration _config;
        protected readonly IStringLocalizer<SharedResource> _stringLocalizer;

        public AppController(SurtusDbContext db, IConfiguration configuration, IStringLocalizer<SharedResource> stringLocalizer) { _db = db; _config = configuration; _stringLocalizer = stringLocalizer; }

        protected Dictionary<string, Dictionary<string, Dictionary<string, object>>> apiModelErrors = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

        protected bool IsModelValid { get => apiModelErrors.Count == 0; }

        protected Dictionary<string, object> AddModelError(string property, string error, string parameter = null, object paramArgs = null)
        {
            if (!apiModelErrors.TryGetValue(property, out var propertyErrors))
            {
                propertyErrors = new Dictionary<string, Dictionary<string, object>>();
                apiModelErrors.Add(property, propertyErrors);
            }
            if (!propertyErrors.TryGetValue(error, out var errorParameters))
            {
                errorParameters = new Dictionary<string, object>();
                propertyErrors.Add(error, errorParameters);
            }
            if (parameter != null && paramArgs != null && !errorParameters.TryAdd(parameter, paramArgs))
            {
                errorParameters[parameter] = paramArgs;
            }
            return errorParameters;
        }

        protected void RemoveModelPropertyParameterError(string property, string error, string parameter, bool emptyRemove = true)
        {
            if (!apiModelErrors.TryGetValue(property, out var propertyErrors) && !propertyErrors.TryGetValue(error, out var errorParameters) && errorParameters.Remove(parameter))
            {
                if (errorParameters.Count == 0 && emptyRemove && propertyErrors.Remove(error))
                {
                    if (propertyErrors.Count == 0)
                    {
                        apiModelErrors.Remove(property);
                    }
                }
            }
        }

        protected void RemoveModelPropertyError(string property, string error)
        {
            if (!apiModelErrors.TryGetValue(property, out var propertyErrors) && propertyErrors.Remove(error))
            {
                if (propertyErrors.Count == 0)
                {
                    apiModelErrors.Remove(property);
                }
            }
        }

        protected void RemoveModelError(string property) => apiModelErrors.Remove(property);

        protected void ValidateFileMaxSize(IFormFile file, string propertyName, long size)
        {
            if (file.Length > 0 && file.Length > size)
            {
                AddModelError(propertyName, "size", "max", size);
            }
        }

        protected T Success<T>(T data, int statusCode = 200)
        {
            HttpContext.Response.StatusCode = statusCode;
            return data;
        }

        protected ApiException Error(string message, int statusCode = 500, int errorCode = -1)
            => new ApiException(message, statusCode, errorCode);

        protected ApiException ValidationsError(string message = "validations")
        {
            ModelState.IterateModelStateErrorParameters(ControllerContext.ActionDescriptor, AddModelError);
            return new ApiException(message, 400, 0, apiModelErrors);
        }

        protected ApiException UnauthorizedError(string message = "unauthorized")
            => new ApiException(message, 401, -1);

        protected ApiException ForbiddenError(string message = "forbidden")
            => new ApiException(message, 403, -1);

        protected ApiException NotFoundError(string message = "notfound")
            => new ApiException(message, 404, -1);

        protected ApiException NotAllowedError(string message = "notallowed")
            => new ApiException(message, 405, -1);
    }
}
