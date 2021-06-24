using Microsoft.AspNetCore.Mvc;
using surtus_api_restful.Middlewares.ApiWrapper.Wrappers;
using System.Threading.Tasks;

namespace surtus_api_restful.Middlewares.ApiWrapper
{
    public class ApiResponseValidationsErrorResult : IActionResult
    {
        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = 400;
            var error = new ApiError("validations")
            {
                ErrorCode = 0,
                ValidationErrors = context.ModelState.ToParameterizedModelError(context.ActionDescriptor)
            };
            await new ObjectResult(error).ExecuteResultAsync(context);
        }
    }

}
