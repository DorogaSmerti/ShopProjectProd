namespace MyFirstProject.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private const string SecretCode = "TopPassword";

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if(!context.Request.Headers.TryGetValue("X-API", out var extractedKey))
        {
            context.Response.StatusCode = 401;
            return;
        }
        if(extractedKey != SecretCode)
        {
            context.Response.StatusCode = 403;
            return;
        }
        await _next(context);
    }
}