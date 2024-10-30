using Microsoft.AspNetCore.Http.HttpResults;
using FluentValidation;
using System.Net;
using System.Text.Json;
using MIS.Models.DTO;

namespace MIS.Middleware
{
    public class CustomExсeptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExсeptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            object response;
            
            switch (exception)
            {
                case ValidationException:
                    status = HttpStatusCode.BadRequest;
                    response = new { status = "Invalid arguments", message = exception.Message };
                    break;

                case KeyNotFoundException:
                    status = HttpStatusCode.NotFound;
                    response = new { status = "Not Found", message = exception.Message };
                    break;

                default:
                    status = HttpStatusCode.InternalServerError;
                    response = new ResponseModel { status = "InternalServiceError", message = "An unexpected error occured" };
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAync(jsonResponse);
        }
    }
}
