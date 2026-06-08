namespace AlaiaStore.Web.ViewModels.Profile;

public class ProfileViewModel
{
    public ProfileUpdateViewModel ProfileUpdate { get; set; } = new();
    public ChangePasswordViewModel ChangePassword { get; set; } = new();
    
    public string Email { get; set; } = string.Empty;
}
