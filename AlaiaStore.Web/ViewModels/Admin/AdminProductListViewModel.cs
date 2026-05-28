using AlaiaStore.Web.DTOs;

namespace AlaiaStore.Web.ViewModels.Admin;

public class AdminProductListViewModel
{
    public IEnumerable<AdminProductListItemViewModel> Products { get; set; } = new List<AdminProductListItemViewModel>();
    public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    public int? SelectedCategoryId { get; set; }
}
