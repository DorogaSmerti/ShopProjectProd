using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstProject.Services;
using MyFirstProject.Models;

namespace MyFirstProject.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]

public class UserController : ApiControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users.Value);
    }

    [HttpGet("roles/{userId}")]
    public async Task<IActionResult> GetUserRolesAsync(string userId)
    {
        var user = await _userService.GetUserRolesAsync(userId);
        if (!user.IsSuccess)
        {
            return HandleFailure(user.Error, user);
        }

        return Ok(user.Value);
    }

    [HttpPost("AddRole")]
    public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleDto addRoleDto)
    {
        var result = await _userService.AddRoleAsync(addRoleDto);
        
        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error, result);
        }

        return Ok(new {message = "Role user added"});
    }

    [HttpDelete("deleteRole")]
    public async Task<IActionResult> DeleteRoleAsync([FromBody] AddRoleDto addRoleDto)
    {
        var result = await _userService.DeleteRoleAsync(addRoleDto);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error, result);
        }

        return Ok(new {message = "Role user deleted"});
    }

    [HttpDelete("deleteUser")]
    public async Task<IActionResult> DeleteUserAsync([FromBody] DeleteUserDto deleteUserDto)
    {
        var result = await _userService.DeleteUserAsync(deleteUserDto);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error, result);
        }

        return Ok(new {message = "User deleted"});
    }
}