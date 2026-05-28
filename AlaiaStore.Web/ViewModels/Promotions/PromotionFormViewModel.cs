using System.ComponentModel.DataAnnotations;

namespace AlaiaStore.Web.ViewModels.Promotions;

public class PromotionFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(0, 100)]
    public decimal? DiscountPercentage { get; set; }

    [Required]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required]
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(7);

    public bool IsActive { get; set; } = true;
}
