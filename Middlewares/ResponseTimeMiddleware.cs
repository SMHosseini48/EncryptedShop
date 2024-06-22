using System.Diagnostics;

namespace MultiLevelEncryptedEshop.Middlewares;

public class ResponseTimeMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseTimeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        context.Response.OnStarting(() => {
            stopwatch.Stop();
            var responseTimeForCompleteRequest = stopwatch.ElapsedMilliseconds;
            context.Response.Headers["ResponseTime"] = responseTimeForCompleteRequest.ToString();
            return Task.CompletedTask;
        });

        await _next(context);
    }
}