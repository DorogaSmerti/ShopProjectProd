using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MyFirstProject.Extensions;

public static class ValidationCatcherExtensions
{
    public static IServiceCollection AddValidationCatcher(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { 
                Field = x.Key, 
                Errors = x.Value.Errors.Select(e => e.ErrorMessage) 
            });

            Log.Warning("⚠️ Validation failed for request {Path}. Errors: {@ValidationErrors}",
            context.HttpContext.Request.Path, errors);

            return new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));

            };
        });
        return services;
    }
}