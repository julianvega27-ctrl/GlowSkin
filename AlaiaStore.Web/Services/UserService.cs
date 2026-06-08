using AlaiaStore.Domain.Entities;
using AlaiaStore.Domain.Interfaces;

namespace AlaiaStore.Web.Services;

public interface IUserService
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> AuthenticateAsync(string email, string password);
    Task<User> RegisterAsync(string firstName, string lastName, string email, string password);
    Task<User> UpdateProfileAsync(int userId, string firstName, string lastName, string? phone, string? profilePictureUrl);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authService;

    public UserService(IUserRepository userRepository, IAuthenticationService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        var user = await _userRepository.GetWithRolesByEmailAsync(email);
        if (user == null)
            return null;

        if (!_authService.VerifyPassword(password, user.PasswordHash))
            return null;

        return user;
    }

    public async Task<User> RegisterAsync(string firstName, string lastName, string email, string password)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
            throw new InvalidOperationException("El correo ya está registrado.");

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PasswordHash = _authService.HashPassword(password),
            IsActive = true
        };

        var createdUser = await _userRepository.AddAsync(user);
        await _userRepository.AssignRoleAsync(createdUser.Id, "Customer");
        return createdUser;
    }

    public async Task<User> UpdateProfileAsync(int userId, string firstName, string lastName, string? phone, string? profilePictureUrl)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("Usuario no encontrado.");

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Phone = phone;

        if (!string.IsNullOrEmpty(profilePictureUrl))
        {
            user.ProfilePictureUrl = profilePictureUrl;
        }

        await _userRepository.UpdateAsync(user);
        return user;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return false;

        if (!_authService.VerifyPassword(currentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = _authService.HashPassword(newPassword);
        await _userRepository.UpdateAsync(user);
        return true;
    }
}
