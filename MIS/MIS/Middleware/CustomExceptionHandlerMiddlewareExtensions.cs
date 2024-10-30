namespace MIS.Middleware
{
    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            return builder.UseMiddleware<CustomExсeptionHandlerMiddleware>();
        }
    }
}
