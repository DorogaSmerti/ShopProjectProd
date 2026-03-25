namespace MyFirstProject.Middleware;

public class DecoratorMiddleware
{
    private readonly RequestDelegate _next;

    public DecoratorMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Append("Api-key", "4135362");
        
        await _next(context);
        
    }
}