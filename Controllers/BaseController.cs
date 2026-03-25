using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MyFirstProject.Services;
using Serilog;

namespace MyFirstProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{

    protected string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    protected IActionResult HandleFailure(Error error, object? context = null)
    {
        Log.Warning("Action failed. ErrorCode: {Code}, Context: {@context}", error.Code, context);
            return error.Code switch
        {
            var code when code.Contains("NotFound") => NotFound(error),
            var code when code.Contains("Unauthorized") => Unauthorized(error),
            _ => BadRequest(error)
        };
    }
}