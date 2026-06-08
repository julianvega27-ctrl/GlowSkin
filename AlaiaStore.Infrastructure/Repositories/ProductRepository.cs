using AlaiaStore.Domain.Entities;
using AlaiaStore.Domain.Interfaces;
using AlaiaStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AlaiaStore.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
    {
        return await _context.Products.Include(p => p.Category).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _context.Products.Include(p => p.Category).Where(p => p.CategoryId == categoryId).ToListAsync();
    }

    public async Task<Product?> GetProductWithCategoryAsync(int id)
    {
        return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
    {
        var normalizedQuery = query.Trim().ToLower();
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Name.ToLower().Contains(normalizedQuery)
                || (p.Description != null && p.Description.ToLower().Contains(normalizedQuery))
                || (p.Ingredients != null && p.Ingredients.ToLower().Contains(normalizedQuery))
                || (p.SkinType != null && p.SkinType.ToLower().Contains(normalizedQuery)))
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetBestSellersAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsBestSeller && p.IsActive)
            .ToListAsync();
    }
}