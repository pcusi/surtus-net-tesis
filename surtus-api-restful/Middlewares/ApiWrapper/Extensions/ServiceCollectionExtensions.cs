using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace surtus_api_restful.Middlewares.ApiWrapper
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureApiWrapperBehavior(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(opts =>
            {
                opts.InvalidModelStateResponseFactory = ctx => new ApiResponseValidationsErrorResult();
            });
        }
    }
}
