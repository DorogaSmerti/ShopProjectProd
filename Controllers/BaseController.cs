using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyFirstProject.Services;

namespace MyFirstProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult HandleFailure(Error error)
    {
            return error.Code switch
        {
            var code when code.Contains("NotFound") => NotFound(error),
            var code when code.Contains("Unauthorized") => Unauthorized(error),
            _ => BadRequest(error)
        };
    }
}