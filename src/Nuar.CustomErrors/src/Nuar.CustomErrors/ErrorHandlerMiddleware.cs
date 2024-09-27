using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nuar.Extensions.CustomErrors;

namespace Nuar.Extensions.CustomErrors
{
    public class ErrorHandlerMiddleware : IMiddleware
    {
        private readonly CustomErrorsOptions _options;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(IOptions<CustomErrorsOptions> options, ILogger<ErrorHandlerMiddleware> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                await HandleErrorAsync(context, exception);
            }
        }

        private Task HandleErrorAsync(HttpContext context, Exception exception)
        {
            var errorCode = "error";
            var statusCode = HttpStatusCode.InternalServerError;
            var message = _options.IncludeExceptionMessage ? exception.Message : "An unexpected error occurred.";

            // Create the response object
            var response = new
            {
                errors = new[]
                {
                    new
                    {
                        Code = errorCode,
                        Message = message,
                        StatusCode = (int)statusCode
                    }
                }
            };

            var payload = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(payload);
        }
    }
}
