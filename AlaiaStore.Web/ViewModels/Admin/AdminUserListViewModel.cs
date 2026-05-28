namespace AlaiaStore.Web.ViewModels.Admin;

public class AdminUserListViewModel
{
    public IEnumerable<AdminUserListItemViewModel> Users { get; set; } = new List<AdminUserListItemViewModel>();
}
