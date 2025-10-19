using Library.Domain.Constants;
using Library.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Infrastructure.Data;

public static class ApplicationDbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        // Ensure database is created
        await dbContext.Database.EnsureCreatedAsync();

        // Seed roles
        await SeedRolesAsync(roleManager);

        // Seed users
        await SeedUsersAsync(userManager);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (var role in UserRoles.AllRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        if (userManager.Users.Any())
            return;

        var users = new[]
        {
            new
            {
                User = new ApplicationUser
                {
                    UserName = "admin@library.com",
                    Email = "admin@library.com",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    MembershipDate = DateTime.UtcNow
                },
                Password = "Admin@123",
                Roles = new[] { UserRoles.Admin }
            },
            new
            {
                User = new ApplicationUser
                {
                    UserName = "librarian@library.com",
                    Email = "librarian@library.com",
                    FirstName = "Librarian",
                    LastName = "User",
                    EmailConfirmed = true,
                    MembershipDate = DateTime.UtcNow
                },
                Password = "Librarian@123",
                Roles = new[] { UserRoles.Librarian }
            },
            new
            {
                User = new ApplicationUser
                {
                    UserName = "member@library.com",
                    Email = "member@library.com",
                    FirstName = "Member",
                    LastName = "User",
                    EmailConfirmed = true,
                    MembershipDate = DateTime.UtcNow
                },
                Password = "Member@123",
                Roles = new[] { UserRoles.Member }
            },
            new
            {
                User = new ApplicationUser
                {
                    UserName = "multiuser@library.com",
                    Email = "multiuser@library.com",
                    FirstName = "Multi",
                    LastName = "Role",
                    EmailConfirmed = true,
                    MembershipDate = DateTime.UtcNow
                },
                Password = "Multi@123",
                Roles = new[] { UserRoles.Librarian, UserRoles.Member }
            }
        };

        foreach (var userData in users)
        {
            var result = await userManager.CreateAsync(userData.User, userData.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRolesAsync(userData.User, userData.Roles);
            }
        }
    }
}