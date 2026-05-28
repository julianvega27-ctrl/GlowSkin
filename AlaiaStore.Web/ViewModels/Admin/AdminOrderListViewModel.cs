namespace AlaiaStore.Web.ViewModels.Admin;

public class AdminOrderListViewModel
{
    public IEnumerable<AdminOrderListItemViewModel> Orders { get; set; } = new List<AdminOrderListItemViewModel>();
}
