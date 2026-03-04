public interface IRateLimitingMiddleware
{
    Task InvokeAsync(HttpContext context);
    Task ClearAll();
}