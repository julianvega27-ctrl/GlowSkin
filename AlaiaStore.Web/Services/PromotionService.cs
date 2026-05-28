using AlaiaStore.Domain.Entities;
using AlaiaStore.Domain.Interfaces;

namespace AlaiaStore.Web.Services;

public interface IPromotionService
{
    Task<decimal?> GetActiveDiscountPercentageAsync(int productId, DateTime nowUtc);
}

public class PromotionService : IPromotionService
{
    private readonly IRepository<ProductPromotion> _productPromotionRepository;
    private readonly IRepository<Promotion> _promotionRepository;

    public PromotionService(IRepository<ProductPromotion> productPromotionRepository, IRepository<Promotion> promotionRepository)
    {
        _productPromotionRepository = productPromotionRepository;
        _promotionRepository = promotionRepository;
    }

    public async Task<decimal?> GetActiveDiscountPercentageAsync(int productId, DateTime nowUtc)
    {
        var productPromotions = await _productPromotionRepository.GetAllAsync();
        var promotionIds = productPromotions
            .Where(pp => pp.ProductId == productId)
            .Select(pp => pp.PromotionId)
            .ToHashSet();

        if (!promotionIds.Any())
        {
            return null;
        }

        var promotions = await _promotionRepository.GetAllAsync();
        var activePromotion = promotions
            .Where(p => promotionIds.Contains(p.Id))
            .Where(p => p.IsActive && p.StartDate <= nowUtc && p.EndDate >= nowUtc)
            .OrderByDescending(p => p.DiscountPercentage ?? 0)
            .FirstOrDefault();

        return activePromotion?.DiscountPercentage;
    }
}
