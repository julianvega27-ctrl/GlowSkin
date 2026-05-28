using AlaiaStore.Domain.Entities;

namespace AlaiaStore.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetAllWithCategoryAsync();
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<Product?> GetProductWithCategoryAsync(int id);
    Task<IEnumerable<Product>> SearchProductsAsync(string query);
}
