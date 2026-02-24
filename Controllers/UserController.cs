using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyFirstProject.Services;
using MyFirstProject.Models;

namespace MyFirstProject.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]

public class UserController : ControllerBase
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
        return Ok(users);
    }

    [HttpGet("roles/{userId}")]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var user = await _userService.GetUserRolesAsync(userId);
        if (user == null)
        {
            return BadRequest(new {message = "Пользователь не найден"});
        }

        return Ok(user);
    }

    [HttpPost("roleChanger")]
    public async Task<IActionResult> RankChanger([FromBody] ChangeRoleDto changeRoleDto)
    {
        var result = await _userService.ChangeRoleAsync(changeRoleDto);
        
        if (result.Status == UserResult.UserNotFound)
        {
            return BadRequest(new {message = "Пользователь не найден"});
        }

        if (result.Status == UserResult.UserHasThisRole)
        {
            return BadRequest(new {message = "Пользователь уже имеет эту роль"});
        }

        if (result.Status == UserResult.Failure)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new {message = "Роль пользователя изменена"});
    }

    [HttpDelete("deleteRole")]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserDto deleteUserDto)
    {
        var result = await _userService.DeleteUserAsync(deleteUserDto);

        if (result == UserResult.UserNotFound)
        {
            return BadRequest(new {message = "Пользователь не найден"});
        }

        return Ok(new {message = "Пользователь удалён"});
    }
}