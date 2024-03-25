using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
namespace Publications.API.Middleware;

    public class ExceptionHandlerMiddleware(RequestDelegate next)
    {
        public Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                return next(httpContext);
            }
            catch (Exception ex)
            {
                return HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var result = JsonSerializer.Serialize(new { error = exception.Message});
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(result);
        }
    }
