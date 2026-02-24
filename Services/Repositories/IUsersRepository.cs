using Microsoft.AspNetCore.Identity;

public interface IUsersRepository
{
    Task<IdentityUser> GetUserByIdAsync(string userId);

}