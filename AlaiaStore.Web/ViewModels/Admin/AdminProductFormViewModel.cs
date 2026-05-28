using AlaiaStore.Web.DTOs;
using AlaiaStore.Web.DTOs.Admin;

namespace AlaiaStore.Web.ViewModels.Admin;

public class AdminProductFormViewModel
{
    public AdminProductEditDto Product { get; set; } = new();
    public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
}
