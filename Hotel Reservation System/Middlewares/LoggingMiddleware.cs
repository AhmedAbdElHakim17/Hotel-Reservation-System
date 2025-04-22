using Microsoft.AspNetCore.Mvc;

namespace HRS_Presentation.Middlewares
{
    public class LoggingMiddleware : Controller
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LoggingMiddleware> logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            logger.LogInformation("[REQUEST] {Method} {Path} ",
                context.Request.Method, context.Request.Path);
            await next(context);
            logger.LogInformation("[RESPONSE] {StatusCode} for {Path} ",
                context.Response.StatusCode, context.Request.Path);
        }
    }
}
