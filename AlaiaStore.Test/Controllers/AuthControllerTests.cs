using AlaiaStore.Domain.Entities;
using AlaiaStore.Web.Controllers;
using AlaiaStore.Web.DTOs;
using AlaiaStore.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace AlaiaStore.Test.Controllers;

[TestClass]
public class AuthControllerTests
{
    private Mock<IUserService> _mockUserService = null!;
    private AuthController _controller = null!;
    private DefaultHttpContext _httpContext = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockUserService = new Mock<IUserService>();
        _httpContext = new DefaultHttpContext();
        
        var authServiceMock = new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationService>();
        authServiceMock
            .Setup(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        var urlHelperMock = new Mock<Microsoft.AspNetCore.Mvc.IUrlHelper>();
        var urlHelperFactoryMock = new Mock<Microsoft.AspNetCore.Mvc.Routing.IUrlHelperFactory>();
        urlHelperFactoryMock.Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>())).Returns(urlHelperMock.Object);

        var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        serviceCollection.AddSingleton(authServiceMock.Object);
        serviceCollection.AddSingleton(urlHelperFactoryMock.Object);
        _httpContext.RequestServices = serviceCollection.BuildServiceProvider();

        _controller = new AuthController(_mockUserService.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = _httpContext },
            TempData = new Mock<ITempDataDictionary>().Object
        };
    }

    [TestMethod]
    public void Register_Get_ReturnsView()
    {
        var result = _controller.Register();
        Assert.IsInstanceOfType<ViewResult>(result);
    }

    [TestMethod]
    public async Task Register_Post_InvalidModel_ReturnsView()
    {
        _controller.ModelState.AddModelError("Error", "Error");
        var dto = new RegisterDto();

        var result = await _controller.Register(dto);

        Assert.IsInstanceOfType<ViewResult>(result);
        var viewResult = result as ViewResult;
        Assert.AreEqual(dto, viewResult?.Model);
    }

    [TestMethod]
    public async Task Register_Post_PasswordsDoNotMatch_ReturnsView()
    {
        var dto = new RegisterDto { Password = "123", ConfirmPassword = "456" };

        var result = await _controller.Register(dto);

        Assert.IsInstanceOfType<ViewResult>(result);
        Assert.IsTrue(_controller.ModelState.ContainsKey("ConfirmPassword"));
    }

    [TestMethod]
    public async Task Register_Post_Success_RedirectsToLogin()
    {
        var dto = new RegisterDto { FirstName = "A", LastName = "B", Email = "a@b.com", Password = "123", ConfirmPassword = "123" };
        _controller.ModelState.Clear();
        _mockUserService.Setup(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new User());

        var result = await _controller.Register(dto);

        Assert.IsInstanceOfType<RedirectToActionResult>(result);
        var redirectResult = result as RedirectToActionResult;
        Assert.AreEqual("Login", redirectResult?.ActionName);
    }

    [TestMethod]
    public void Login_Get_ReturnsView()
    {
        var result = _controller.Login();
        Assert.IsInstanceOfType<ViewResult>(result);
    }

    [TestMethod]
    public async Task Login_Post_InvalidModel_ReturnsView()
    {
        _controller.ModelState.AddModelError("Error", "Error");
        var dto = new LoginDto();

        var result = await _controller.Login(dto);

        Assert.IsInstanceOfType<ViewResult>(result);
    }

    [TestMethod]
    public async Task Login_Post_InvalidCredentials_ReturnsView()
    {
        var dto = new LoginDto { Email = "a@b.com", Password = "123" };
        _mockUserService.Setup(s => s.AuthenticateAsync(dto.Email, dto.Password)).ReturnsAsync((User?)null);

        var result = await _controller.Login(dto);

        Assert.IsInstanceOfType<ViewResult>(result);
        Assert.IsFalse(_controller.ModelState.IsValid);
    }

    [TestMethod]
    public async Task Login_Post_Success_RedirectsToHome()
    {
        var dto = new LoginDto { Email = "a@b.com", Password = "123" };
        var user = new User { Id = 1, FirstName = "A", Email = "a@b.com", UserRoles = new List<UserRole>() };
        _mockUserService.Setup(s => s.AuthenticateAsync(dto.Email, dto.Password)).ReturnsAsync(user);

        var result = await _controller.Login(dto);

        Assert.IsInstanceOfType<RedirectToActionResult>(result);
        var redirectResult = result as RedirectToActionResult;
        Assert.AreEqual("Index", redirectResult?.ActionName);
        Assert.AreEqual("Home", redirectResult?.ControllerName);
    }
}
