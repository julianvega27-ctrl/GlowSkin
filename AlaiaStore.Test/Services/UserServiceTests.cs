using AlaiaStore.Domain.Entities;
using AlaiaStore.Domain.Interfaces;
using AlaiaStore.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AlaiaStore.Test.Services;

[TestClass]
public class UserServiceTests
{
    private Mock<IUserRepository> _mockUserRepo = null!;
    private Mock<IAuthenticationService> _mockAuthService = null!;
    private UserService _userService = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockAuthService = new Mock<IAuthenticationService>();
        _userService = new UserService(_mockUserRepo.Object, _mockAuthService.Object);
    }

    [TestMethod]
    public async Task GetUserByEmailAsync_ReturnsUser()
    {
        var user = new User { Email = "test@test.com" };
        _mockUserRepo.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync(user);

        var result = await _userService.GetUserByEmailAsync("test@test.com");

        Assert.AreEqual(user, result);
    }

    [TestMethod]
    public async Task AuthenticateAsync_InvalidEmail_ReturnsNull()
    {
        _mockUserRepo.Setup(r => r.GetWithRolesByEmailAsync("wrong@test.com")).ReturnsAsync((User?)null);

        var result = await _userService.AuthenticateAsync("wrong@test.com", "pass");

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task AuthenticateAsync_InvalidPassword_ReturnsNull()
    {
        var user = new User { Email = "test@test.com", PasswordHash = "hash" };
        _mockUserRepo.Setup(r => r.GetWithRolesByEmailAsync("test@test.com")).ReturnsAsync(user);
        _mockAuthService.Setup(s => s.VerifyPassword("wrong", "hash")).Returns(false);

        var result = await _userService.AuthenticateAsync("test@test.com", "wrong");

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsUser()
    {
        var user = new User { Email = "test@test.com", PasswordHash = "hash" };
        _mockUserRepo.Setup(r => r.GetWithRolesByEmailAsync("test@test.com")).ReturnsAsync(user);
        _mockAuthService.Setup(s => s.VerifyPassword("correct", "hash")).Returns(true);

        var result = await _userService.AuthenticateAsync("test@test.com", "correct");

        Assert.AreEqual(user, result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task RegisterAsync_ExistingEmail_ThrowsException()
    {
        _mockUserRepo.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync(new User());

        await _userService.RegisterAsync("A", "B", "test@test.com", "pass");
    }

    [TestMethod]
    public async Task RegisterAsync_NewUser_ReturnsCreatedUser()
    {
        _mockUserRepo.Setup(r => r.GetByEmailAsync("new@test.com")).ReturnsAsync((User?)null);
        _mockAuthService.Setup(s => s.HashPassword("pass")).Returns("hash");
        _mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(new User { Id = 1, Email = "new@test.com", PasswordHash = "hash" });

        var result = await _userService.RegisterAsync("A", "B", "new@test.com", "pass");

        Assert.IsNotNull(result);
        Assert.AreEqual("new@test.com", result.Email);
        Assert.AreEqual("hash", result.PasswordHash);
        _mockUserRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);

    }
}
