using Microsoft.AspNetCore.Identity;
using MyFirstProject.Data;

namespace MyFirstProject.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly AppDbContext _context;

    public UsersRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IdentityUser> GetUserByIdAsync(string userId)
    {
        return await _context.Users.FindAsync(userId);
    }
}