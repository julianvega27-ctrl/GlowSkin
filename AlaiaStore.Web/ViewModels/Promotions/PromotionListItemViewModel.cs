namespace AlaiaStore.Web.ViewModels.Promotions;

public class PromotionListItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal? DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}
