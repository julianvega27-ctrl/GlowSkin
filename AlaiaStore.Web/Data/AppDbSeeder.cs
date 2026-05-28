using AlaiaStore.Domain.Entities;
using AlaiaStore.Infrastructure.Data;
using AlaiaStore.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace AlaiaStore.Web.Data;

public static class AppDbSeeder
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        await context.Database.MigrateAsync();

        if (!await context.Roles.AnyAsync())
        {
            var adminRole = new Role { Name = "Admin" };
            var customerRole = new Role { Name = "Customer" };

            await context.Roles.AddRangeAsync(adminRole, customerRole);
            await context.SaveChangesAsync();
        }

        var authService = serviceProvider.GetRequiredService<IAuthenticationService>();
        var roleAdmin = await context.Roles.FirstAsync(r => r.Name == "Admin");
        
        var adminUser = await context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Email == "admin@alaiastore.com");

        if (adminUser == null)
        {
            adminUser = new User
            {
                FirstName = "Admin",
                LastName = "System",
                Email = "admin@alaiastore.com",
                PasswordHash = authService.HashPassword("Admin123!"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }

        if (!adminUser.UserRoles.Any(ur => ur.RoleId == roleAdmin.Id))
        {
            await context.UserRoles.AddAsync(new UserRole { UserId = adminUser.Id, RoleId = roleAdmin.Id });
            await context.SaveChangesAsync();
        }
    }
}
