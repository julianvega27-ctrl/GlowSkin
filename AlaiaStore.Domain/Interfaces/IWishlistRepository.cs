using AlaiaStore.Domain.Entities;

namespace AlaiaStore.Domain.Interfaces;

public interface IWishlistRepository : IRepository<Wishlist>
{
    Task<Wishlist?> GetWishlistByUserIdAsync(int userId);
    Task<bool> HasProductInWishlistAsync(int userId, int productId);
}