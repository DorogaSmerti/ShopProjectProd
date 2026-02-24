using Microsoft.AspNetCore.Identity;
using MyFirstProject.Models;

namespace MyFirstProject.Services;

public interface IUserService
{
    Task<List<IdentityUser>> GetAllUsersAsync();
    Task<List<string>?> GetUserRolesAsync(string user);
    Task<(UserResult Status, IdentityResult? Errors)> ChangeRoleAsync(ChangeRoleDto changeRoleDto);
    Task<UserResult> DeleteUserAsync(DeleteUserDto deleteUserDto);
}