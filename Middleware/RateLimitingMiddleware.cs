using System.Collections.Concurrent;
using MyFirstProject.BackgroundServices;

namespace MyFirstProject.Middleware;

public class RateLimitingMiddleware : IRateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly IRateLimitStore _store;

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, IRateLimitStore  store)
    {
        _next = next;
        _logger = logger;
        _store = store;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string ipKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var result = _store.IncrementAndGet(ipKey);

        if(result > 5)
        {
            context.Response.StatusCode = 429;
            return;
        }
        
        await _next(context);
    }

    public async Task ClearAll()
    {
        _store.CleanAll();
    }
}