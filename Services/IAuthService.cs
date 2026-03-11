using MyFirstProject.Models;

namespace MyFirstProject.Services;

public interface IAuthService
{
    Task<Result<string>> LoginAsync(LoginDto loginDto);
    Task<Result<bool>> RegisterAsync(RegisterDto registerDto);
}