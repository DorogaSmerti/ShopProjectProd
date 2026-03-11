using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MyFirstProject.Data;
using MyFirstProject.Models;
using Serilog;

namespace MyFirstProject.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _config;

    public AuthService(UserManager<IdentityUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<Result<bool>> RegisterAsync(RegisterDto registerDto)
    {
        var userExist = await _userManager.FindByNameAsync(registerDto.Username);

        if(userExist != null)
        {
            return Result<bool>.Failure(DomainErrors.User.UserAlreadyExists);
        }

        var emailExist = await _userManager.FindByEmailAsync(registerDto.Email);

        if(emailExist != null)
        {
            return Result<bool>.Failure(DomainErrors.User.EmailAlreadyExists);
        }

        var user = new IdentityUser
        {
            UserName =  registerDto.Username,
            Email = registerDto.Email
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return Result<bool>.Failure(DomainErrors.User.UserCannotRegister);
        }

        await _userManager.AddToRoleAsync(user, "User");


        return Result<bool>.Success(true);
    }

    public async Task<Result<string>> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.Username);

        if(user == null)
        {
            return Result<string>.Failure(DomainErrors.User.UserNotFound);
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var passwordCheck = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if(passwordCheck == false)
        {
            return Result<string>.Failure(DomainErrors.User.PasswordOrUsernameDoesNotMatch);
        }

        var Claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        foreach(var role in userRoles)
        {
            Claims.Add (new Claim(ClaimTypes.Role, role));
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

        var SingKey = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken
        (
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            expires: DateTime.UtcNow.AddHours(1),
            claims: Claims,
            signingCredentials: SingKey
        );

        return Result<string>.Success(new JwtSecurityTokenHandler().WriteToken(token));
    }
}