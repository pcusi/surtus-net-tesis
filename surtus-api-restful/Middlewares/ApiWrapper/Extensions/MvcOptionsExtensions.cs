using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace surtus_api_restful.Middlewares.ApiWrapper
{
    public static class MvcOptionsExtensions
    {
        public static void AddApiWrapperFormatter(this MvcOptions options, IServiceCollection services)
        {
            var messageProvider = options.ModelBindingMessageProvider;

            messageProvider.SetAttemptedValueIsInvalidAccessor((v, f) => "value");
            messageProvider.SetMissingBindRequiredValueAccessor(v => "required");
            messageProvider.SetMissingKeyOrValueAccessor(() => "required");
            messageProvider.SetMissingRequestBodyRequiredValueAccessor(() => "required");
            messageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(v => "value");
            messageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() => "value");
            messageProvider.SetNonPropertyValueMustBeANumberAccessor(() => "value");
            messageProvider.SetUnknownValueIsInvalidAccessor(v => "value");
            messageProvider.SetValueIsInvalidAccessor(v => "value");
            messageProvider.SetValueMustBeANumberAccessor(v => "value");
            messageProvider.SetValueMustNotBeNullAccessor(v => "required");

            var index = 0;
            SystemTextJsonOutputFormatter jsonOutputFormatter = null;
            foreach (var outputFormatter in options.OutputFormatters)
            {
                if (outputFormatter is SystemTextJsonOutputFormatter)
                {
                    jsonOutputFormatter = (SystemTextJsonOutputFormatter)outputFormatter;
                    break;
                }
                ++index;
            }

            if (index < options.OutputFormatters.Count)
            {
                options.OutputFormatters.RemoveAt(index);
                options.OutputFormatters.Insert(index, new ApiWrapperOutputFormatter(jsonOutputFormatter.SerializerOptions));
            }
            else
            {
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    var jsonOptions = serviceProvider.GetRequiredService<IOptions<JsonOptions>>();

                    options.OutputFormatters.Add(new ApiWrapperOutputFormatter(jsonOptions.Value.JsonSerializerOptions));
                }
            }
        }
    }
}
