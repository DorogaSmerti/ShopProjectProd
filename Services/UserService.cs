using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyFirstProject.Models;

namespace MyFirstProject.Services;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;

    public UserService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<IdentityUser>> GetAllUsersAsync()
    {
        return await _userManager.Users.ToListAsync();
    }

    public async Task<List<string>?> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null;
        }
        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    public async Task<(UserResult Status, IdentityResult? Errors)> ChangeRoleAsync(ChangeRoleDto changeRoleDto)
    {
        var user = await _userManager.FindByNameAsync(changeRoleDto.UserName);
        if (user == null)
        {
            return (UserResult.UserNotFound, null);
        }

        if (await _userManager.IsInRoleAsync(user, changeRoleDto.Role))
        {
            return (UserResult.UserHasThisRole, null);
        }

        var result = await _userManager.AddToRoleAsync(user, changeRoleDto.Role);
        if (result.Succeeded)
        {
            return (UserResult.Success, null);
        }
        return (UserResult.Failure, result);
    }

    public async Task<UserResult> DeleteUserAsync(DeleteUserDto deleteUserDto)
    {
        var user = await _userManager.FindByIdAsync(deleteUserDto.UserId);
        if (user == null)
        {
            return UserResult.UserNotFound;
        }

        await _userManager.DeleteAsync(user);
        return UserResult.Success;
    }
}