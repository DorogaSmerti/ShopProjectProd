using System.Diagnostics;
using Serilog;

namespace MyFirstProject.Middleware;

public class LoggerMiddleware
{
    private readonly RequestDelegate _next;
    public LoggerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var time = Stopwatch.StartNew();

        await _next(context);

        time.Stop();

        Log.Information($"Request: {context.Request.Path} time: {time.ElapsedMilliseconds}");
    }
}