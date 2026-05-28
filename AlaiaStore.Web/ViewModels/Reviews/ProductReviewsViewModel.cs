using AlaiaStore.Web.DTOs;

namespace AlaiaStore.Web.ViewModels.Reviews;

public class ProductReviewsViewModel
{
    public ProductDto Product { get; set; } = new();
    public IEnumerable<ReviewListItemViewModel> Reviews { get; set; } = new List<ReviewListItemViewModel>();
    public ReviewCreateDto NewReview { get; set; } = new();
}
