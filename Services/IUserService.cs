using Microsoft.AspNetCore.Identity;
using MyFirstProject.Models;

namespace MyFirstProject.Services;

public interface IUserService
{
    Task<Result<List<IdentityUser>>> GetAllUsersAsync();
    Task<Result<List<string>?>> GetUserRolesAsync(string userId);
    Task<Result<IdentityUser>> ChangeRoleAsync(ChangeRoleDto changeRoleDto);
    Task<Result<IdentityUser>> DeleteUserAsync(DeleteUserDto deleteUserDto);
}