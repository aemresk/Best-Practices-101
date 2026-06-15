using System.Diagnostics;

namespace BestPracticesApi.Common.Middleware;

// 08. Custom Middleware — her isteğin method, path, status ve süresini loglar
public sealed class RequestLoggingMiddleware(
    RequestDelegate                   next,
    ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        finally
        {
            sw.Stop();

            var level = context.Response.StatusCode >= 500 ? LogLevel.Error : LogLevel.Information;

            logger.Log(level,
                "[{Method}] {Path}{Query} → {Status} ({Ms}ms)",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                context.Response.StatusCode,
                sw.ElapsedMilliseconds);
        }
    }
}
