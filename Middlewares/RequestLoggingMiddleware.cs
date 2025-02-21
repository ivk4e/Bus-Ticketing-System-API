using Serilog;
using System.Diagnostics;
using System.Text.Json;

namespace BusifyAPI.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly string[] ExcludedPaths =
        {
            "/swagger", "/favicon.ico", "/_framework", "/vs/browserLink",
            "/_vs/browserLink", "/.well-known", "/health", "/metrics"
        };

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLower();

            // Пропускаме ненужни заявки
            if (ExcludedPaths.Any(p => path.StartsWith(p)))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            var requestMethod = context.Request.Method;
            var requestPath = context.Request.Path;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var logObject = new
                {
                    Method = requestMethod,
                    Path = requestPath,
                    context.Response.StatusCode,
                    DurationMs = stopwatch.ElapsedMilliseconds
                };

                string logMessage = JsonSerializer.Serialize(logObject, new JsonSerializerOptions { WriteIndented = true });

                if (context.Response.StatusCode >= 400)
                {
                    Log.Warning(logMessage);
                }
                else
                {
                    Log.Information(logMessage);
                }
            }
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}