using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AlaiaStore.Web.ViewModels.Profile;

public class ProfileUpdateViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string LastName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public IFormFile? ImageFile { get; set; }

    public string? CurrentProfilePictureUrl { get; set; }
}
