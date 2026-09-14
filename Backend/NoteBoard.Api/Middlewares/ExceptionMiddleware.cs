using System;
using System.Net;
using System.Text.Json;
using NoteBoard.Api.Common.Response;
using NoteBoard.Application.Common.Messages;

namespace NoteBoard.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, ILogger<ExceptionMiddleware> logger)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, logger, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext httpContext, ILogger logger, Exception ex)
        {
            logger.LogError(ex.Message);

            var statusCode = (int)HttpStatusCode.InternalServerError;
            var contentType = "application/json";
            var responseContent = new ApiResponse<object>(default, false, ApplicationMessages.InternalError);
            
            var jsonResult = JsonSerializer.Serialize(responseContent);

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = contentType;
            await httpContext.Response.WriteAsync(jsonResult);
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}