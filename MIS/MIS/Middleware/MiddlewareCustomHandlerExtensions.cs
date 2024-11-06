using Microsoft.AspNetCore.Diagnostics;

namespace MIS.Middleware
{
    public static class MiddlewareCustomHandlerExtensions
    {
        public static void UseMiddlewareHandlerException(this IApplicationBuilder app)
        {
            app.UseMiddleware<MiddlewareCustomHandler>();
        }
    }
}
