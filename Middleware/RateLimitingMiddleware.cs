using MyFirstProject.BackgroundServices;
using Serilog;

namespace MyFirstProject.Middleware;

public class RateLimitingMiddleware : IRateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimitStore _store;

    public RateLimitingMiddleware(RequestDelegate next, IRateLimitStore  store)
    {
        _next = next;
        _store = store;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string ipKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var result = _store.IncrementAndGet(ipKey);

        if(result > 5)
        {
            Log.Information("IP {ip} has exceeded the rate limit with {count} requests.", ipKey, result);
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