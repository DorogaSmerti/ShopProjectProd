using Microsoft.AspNetCore.Identity;
using MyFirstProject.Models;

namespace MyFirstProject.Services;

public interface IUserService
{
    Task<Result<List<IdentityUser>>> GetAllUsersAsync();
    Task<Result<List<string>?>> GetUserRolesAsync(string userId);
    Task<Result<IdentityUser>> AddRoleAsync(AddRoleDto addRoleDto);
    Task<Result<IdentityUser>> DeleteRoleAsync(AddRoleDto addRoleDto);
    Task<Result<IdentityUser>> DeleteUserAsync(DeleteUserDto deleteUserDto);
}