using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyFirstProject.Data;
using Serilog;

namespace MyFirstProject.Extensions;

public static class PrepareDatabaseClass
{
    public static async Task PrepareDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { "Admin", "User", "Manager" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            await SeedAdminAsync(userManager);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while preparing the database");
        }
    }

    private static async Task SeedAdminAsync(UserManager<IdentityUser> userManager)
    {
        var adminName = "Admin";
        var adminEmail = "admin@example.com";
        var adminPassword = "Admin123!";

        var adminUser = await userManager.FindByNameAsync(adminName);
        
        if (adminUser == null)
        {
            adminUser = new IdentityUser { UserName = adminName, Email = adminEmail };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Log.Error("Error when creating admin: {Errors}", errors);
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}