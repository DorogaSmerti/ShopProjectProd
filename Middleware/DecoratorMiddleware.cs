namespace MyFirstProject.Middleware;

public class DecoratorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DecoratorMiddleware> _logger;

    public DecoratorMiddleware(RequestDelegate next, ILogger<DecoratorMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Append("Api-key", "4135362");
        
        await _next(context);
        
    }
}