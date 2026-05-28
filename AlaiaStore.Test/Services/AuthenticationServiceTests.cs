using AlaiaStore.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlaiaStore.Test.Services;

[TestClass]
public class AuthenticationServiceTests
{
    private AuthenticationService _authService = null!;

    [TestInitialize]
    public void Setup()
    {
        _authService = new AuthenticationService();
    }

    [TestMethod]
    public void HashPassword_ReturnsNonEmptyString()
    {
        var password = "MySecurePassword123!";
        var hash = _authService.HashPassword(password);

        Assert.IsFalse(string.IsNullOrEmpty(hash));
        Assert.AreNotEqual(password, hash);
    }

    [TestMethod]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        var password = "MySecurePassword123!";
        var hash = _authService.HashPassword(password);

        var result = _authService.VerifyPassword(password, hash);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void VerifyPassword_IncorrectPassword_ReturnsFalse()
    {
        var password = "MySecurePassword123!";
        var hash = _authService.HashPassword(password);

        var result = _authService.VerifyPassword("WrongPassword!", hash);

        Assert.IsFalse(result);
    }
}
