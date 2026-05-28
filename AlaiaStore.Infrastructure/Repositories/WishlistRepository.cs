using AlaiaStore.Domain.Entities;
using AlaiaStore.Domain.Interfaces;
using AlaiaStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AlaiaStore.Infrastructure.Repositories;

public class WishlistRepository : Repository<Wishlist>, IWishlistRepository
{
    public WishlistRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Wishlist?> GetWishlistByUserIdAsync(int userId)
    {
        return await _context.Wishlists
            .Include(w => w.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(w => w.UserId == userId);
    }

    public async Task<bool> HasProductInWishlistAsync(int userId, int productId)
    {
        var wishlist = await GetWishlistByUserIdAsync(userId);
        if (wishlist == null) return false;

        return wishlist.Items.Any(i => i.ProductId == productId);
    }
}