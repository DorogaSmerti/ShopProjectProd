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
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users.Value);
    }

    [HttpGet("roles/{userId}")]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var user = await _userService.GetUserRolesAsync(userId);
        if (!user.IsSuccess)
        {
            return HandleFailure(user.Error, user);
        }

        return Ok(user.Value);
    }

    [HttpPost("rankChanger")]
    public async Task<IActionResult> RankChanger([FromBody] AddRoleDto AddRoleDto)
    {
        var result = await _userService.AddRoleAsync(AddRoleDto);
        
        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error, result);
        }

        return Ok(new {message = "Роль пользователя изменена"});
    }

    [HttpDelete("deleteRole")]
    public async Task<IActionResult> DeleteRole([FromBody] AddRoleDto addRoleDto)
    {
        var result = await _userService.DeleteRoleAsync(addRoleDto);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error, result);
        }

        return Ok(new {message = "Роль пользователя удалена"});
    }

    [HttpDelete("deleteUser")]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserDto deleteUserDto)
    {
        var result = await _userService.DeleteUserAsync(deleteUserDto);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error, result);
        }

        return Ok(new {message = "Пользователь удалён"});
    }
}