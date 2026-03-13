using Microsoft.AspNetCore.Mvc;
using MyFirstProject.Models;
using MyFirstProject.Services;

namespace MyFirstProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
       var result = await _authService.RegisterAsync(registerDto);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error, result);
        }

        return Ok(new { message = "User created successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error, result);
        }

        return Ok(result.Value);
    }
}