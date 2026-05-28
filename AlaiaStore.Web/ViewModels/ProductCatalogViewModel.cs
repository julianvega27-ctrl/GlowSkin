using AlaiaStore.Web.DTOs;

namespace AlaiaStore.Web.ViewModels;

public class ProductCatalogViewModel
{
    public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
    public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    public int? SelectedCategoryId { get; set; }
}