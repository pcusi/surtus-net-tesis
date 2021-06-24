using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using surtus_api_restful.Middlewares.ApiWrapper.Wrappers;
using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace surtus_api_restful.Middlewares.ApiWrapper
{
    /// 26/01/2020: https://github.com/aspnet/AspNetCore/blob/master/src/Mvc/Mvc.Core/src/Formatters/SystemTextJsonOutputFormatter.cs
    /// <summary>
    /// A custom <see cref="TextOutputFormatter"/> for JSON content that uses <see cref="JsonSerializer"/> based on <see cref="SystemTextJsonOutputFormatter">.
    /// </summary>
    public class ApiWrapperOutputFormatter : TextOutputFormatter
    {
        public JsonSerializerOptions SerializerOptions { get; }

        public ApiWrapperOutputFormatter(JsonSerializerOptions jsonSerializerOptions)
        {
            SerializerOptions = jsonSerializerOptions;

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);

            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/json"));
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/*+json"));
        }

        internal static SystemTextJsonOutputFormatter CreateFormatter(JsonOptions jsonOptions)
        {
            var jsonSerializerOptions = jsonOptions.JsonSerializerOptions;

            if (jsonSerializerOptions.Encoder is null)
            {
                jsonSerializerOptions = jsonSerializerOptions.Copy(JavaScriptEncoder.UnsafeRelaxedJsonEscaping);
            }

            return new SystemTextJsonOutputFormatter(jsonSerializerOptions);
        }

        public sealed override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (selectedEncoding == null) { throw new ArgumentNullException(nameof(selectedEncoding)); }

            var httpContext = context.HttpContext;

            var writeStream = GetWriteStream(httpContext, selectedEncoding);
            try
            {
                Type constructed = typeof(ApiResponse<>).MakeGenericType(new[] { context.Object?.GetType() ?? context.ObjectType ?? typeof(object) });
                var instance = Activator.CreateInstance(constructed, new[] { httpContext.Response.StatusCode, (httpContext.Response.StatusCode >= 200 && httpContext.Response.StatusCode < 300) ? "success" : "error", context.Object });

                await JsonSerializer.SerializeAsync(writeStream, instance, constructed, SerializerOptions);

                if (writeStream is TranscodingWriteStream transcodingStream)
                {
                    await transcodingStream.FinalWriteAsync(CancellationToken.None);
                }
                await writeStream.FlushAsync();
            }
            finally
            {
                if (writeStream is TranscodingWriteStream transcodingStream)
                {
                    await transcodingStream.DisposeAsync();
                }
            }
        }

        private Stream GetWriteStream(HttpContext httpContext, Encoding selectedEncoding)
        {
            if (selectedEncoding.CodePage == Encoding.UTF8.CodePage)
            {
                return httpContext.Response.Body;
            }

            return new TranscodingWriteStream(httpContext.Response.Body, selectedEncoding);
        }
    }

}
