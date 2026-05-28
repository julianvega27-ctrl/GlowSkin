using AlaiaStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AlaiaStore.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        await context.Database.MigrateAsync();

        if (!context.Roles.Any())
        {
            await context.Roles.AddRangeAsync(
                new Role { Name = "Admin" },
                new Role { Name = "Customer" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Categories.Any())
        {
            await context.Categories.AddRangeAsync(
                new Category { Name = "Limpiadores", Description = "Productos para limpieza facial" },
                new Category { Name = "Serums", Description = "Serums hidratantes y reparadores" },
                new Category { Name = "Protectores Solares", Description = "Protección UV para la piel" },
                new Category { Name = "Hidratantes", Description = "Cremas y lociones hidratantes" },
                new Category { Name = "Mascarillas", Description = "Mascarillas faciales" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Products.Any())
        {
            var categoriaLimpiadores = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Limpiadores");
            var categoriaSerums = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Serums");
            var categoriaProtectores = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Protectores Solares");
            var categoriaHidratantes = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Hidratantes");

            if(categoriaLimpiadores != null && categoriaSerums != null && categoriaProtectores != null && categoriaHidratantes != null)
            {
                await context.Products.AddRangeAsync(
                    new Product { CategoryId = categoriaLimpiadores.Id, Name = "Purifying Cleanser", Price = 32.00M, Stock = 50, Description = "Gentle daily cleanser.", MainImageUrl = "/images/products/cleanser.jpg" },
                    new Product { CategoryId = categoriaSerums.Id, Name = "Hydrating Serum", Price = 45.00M, Stock = 30, Description = "Deep hydration serum with HA.", MainImageUrl = "/images/products/serum.jpg" },
                    new Product { CategoryId = categoriaProtectores.Id, Name = "Daily Sunscreen SPF 50", Price = 38.00M, Stock = 40, Description = "Invisible finish UV protection.", MainImageUrl = "/images/products/sunscreen.jpg" },
                    new Product { CategoryId = categoriaHidratantes.Id, Name = "Botanical Night Cream", Price = 58.00M, Stock = 20, Description = "Rich overnight repair cream.", MainImageUrl = "/images/products/cream.jpg" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
