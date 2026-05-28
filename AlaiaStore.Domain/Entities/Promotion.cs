using AlaiaStore.Domain.Common;

namespace AlaiaStore.Domain.Entities;

public class Promotion : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ProductPromotion> ProductPromotions { get; set; } = new List<ProductPromotion>();
}
