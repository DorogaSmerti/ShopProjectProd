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

    public async Task<Result<List<IdentityUser>>> GetAllUsersAsync()
    {
        var result = await _userManager.Users.ToListAsync();
        return Result<List<IdentityUser>>.Success(result);
    }

    public async Task<Result<List<string>?>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<List<string>?>.Failure(DomainErrors.User.UserNotFound);
        }
        var roles = await _userManager.GetRolesAsync(user);
        
        return Result<List<string>?>.Success(roles.ToList());
    }

    public async Task<Result<IdentityUser>> ChangeRoleAsync(ChangeRoleDto changeRoleDto)
    {
        var user = await _userManager.FindByIdAsync(changeRoleDto.UserId);
        if (user == null)
        {
            return Result<IdentityUser>.Failure(DomainErrors.User.UserNotFound);
        }

        if (await _userManager.IsInRoleAsync(user, changeRoleDto.Role))
        {
            return Result<IdentityUser>.Failure(DomainErrors.User.UserHasThisRole);
        }

        var result = await _userManager.AddToRoleAsync(user, changeRoleDto.Role);
        if (result.Succeeded)
        {
            return Result<IdentityUser>.Success(user);
        }
        return Result<IdentityUser>.Failure(DomainErrors.User.UserRoleChangeFailed);
    }

    public async Task<Result<IdentityUser>> DeleteUserAsync(DeleteUserDto deleteUserDto)
    {
        var user = await _userManager.FindByIdAsync(deleteUserDto.UserId);
        if (user == null)
        {
            return Result<IdentityUser>.Failure(DomainErrors.User.UserNotFound);
        }

        await _userManager.DeleteAsync(user);
        return Result<IdentityUser>.Success();
    }
}