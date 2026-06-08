using AlaiaStore.Web.Services;
using AlaiaStore.Web.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AlaiaStore.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IUserService _userService;
    private readonly IImageService _imageService;

    public ProfileController(IUserService userService, IImageService imageService)
    {
        _userService = userService;
        _imageService = imageService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Auth");

        var user = await _userService.GetUserByEmailAsync(userEmail);
        if (user == null) return NotFound();

        var viewModel = new ProfileViewModel
        {
            Email = user.Email,
            ProfileUpdate = new ProfileUpdateViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                CurrentProfilePictureUrl = user.ProfilePictureUrl
            }
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Auth");

        var user = await _userService.GetUserByEmailAsync(userEmail);
        if (user == null) return NotFound();

        if (!ModelState.IsValid)
        {
            model.Email = user.Email;
            model.ProfileUpdate.CurrentProfilePictureUrl = user.ProfilePictureUrl;
            return View("Index", model);
        }

        string? newProfilePictureUrl = null;
        if (model.ProfileUpdate.ImageFile != null)
        {
            newProfilePictureUrl = await _imageService.SaveImageAsync(model.ProfileUpdate.ImageFile, "profiles");
        }

        await _userService.UpdateProfileAsync(user.Id, model.ProfileUpdate.FirstName, model.ProfileUpdate.LastName, model.ProfileUpdate.Phone, newProfilePictureUrl);

        TempData["SuccessMessage"] = "Perfil actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ProfileViewModel model)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Auth");

        var user = await _userService.GetUserByEmailAsync(userEmail);
        if (user == null) return NotFound();

        if (!ModelState.IsValid)
        {
            model.Email = user.Email;
            model.ProfileUpdate = new ProfileUpdateViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                CurrentProfilePictureUrl = user.ProfilePictureUrl
            };
            return View("Index", model);
        }

        var success = await _userService.ChangePasswordAsync(user.Id, model.ChangePassword.CurrentPassword, model.ChangePassword.NewPassword);

        if (!success)
        {
            ModelState.AddModelError("ChangePassword.CurrentPassword", "La contraseña actual es incorrecta.");
            model.Email = user.Email;
            model.ProfileUpdate = new ProfileUpdateViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                CurrentProfilePictureUrl = user.ProfilePictureUrl
            };
            return View("Index", model);
        }

        TempData["SuccessMessage"] = "Contraseña cambiada exitosamente.";
        return RedirectToAction(nameof(Index));
    }
}
