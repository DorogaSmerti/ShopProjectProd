using System.Diagnostics;

namespace MyFirstProject.Middleware;

public class LoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggerMiddleware> _logger;
    public LoggerMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var time = Stopwatch.StartNew();

        await _next(context);

        time.Stop();

        _logger.LogInformation($"Request: {context.Request.Path} time: {time.ElapsedMilliseconds}");
    }
}