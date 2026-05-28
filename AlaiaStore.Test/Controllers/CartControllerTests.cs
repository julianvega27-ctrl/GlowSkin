using AlaiaStore.Domain.Entities;
using AlaiaStore.Infrastructure.Data;
using AlaiaStore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;

namespace AlaiaStore.Test.Controllers;

[TestClass]
public class CartControllerTests
{
    private ApplicationDbContext _context = null!;
    private CartController _controller = null!;
    private Mock<HttpContext> _mockHttpContext = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new ApplicationDbContext(options);

        // Setup mock user
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _mockHttpContext = new Mock<HttpContext>();
        _mockHttpContext.Setup(c => c.User).Returns(claimsPrincipal);

        var tempData = new TempDataDictionary(_mockHttpContext.Object, Mock.Of<ITempDataProvider>());

        _controller = new CartController(_context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
            },
            TempData = tempData
        };
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public async Task Index_NoCartExists_CreatesCartAndReturnsView()
    {
        var result = await _controller.Index();

        Assert.IsInstanceOfType<ViewResult>(result);
        var viewResult = result as ViewResult;
        var cart = viewResult?.Model as Cart;
        
        Assert.IsNotNull(cart);
        Assert.AreEqual(1, cart.UserId);
        Assert.AreEqual(1, _context.Carts.Count());
    }

    [TestMethod]
    public async Task Add_ProductExists_AddsToCartAndRedirects()
    {
        var product = new Product { Id = 1, Name = "Test Product", Price = 100m };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var result = await _controller.Add(1, 2);

        Assert.IsInstanceOfType<RedirectToActionResult>(result);
        var redirectResult = result as RedirectToActionResult;
        Assert.AreEqual("Index", redirectResult?.ActionName);

        var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync();
        Assert.IsNotNull(cart);
        Assert.AreEqual(1, cart.Items.Count);
        Assert.AreEqual(2, cart.Items.First().Quantity);
        Assert.AreEqual(100m, cart.Items.First().UnitPrice);
    }

    [TestMethod]
    public async Task Remove_ItemExists_RemovesItemAndRedirects()
    {
        var cart = new Cart { Id = 1, UserId = 1 };
        var item = new CartItem { Id = 1, CartId = 1, ProductId = 1, Quantity = 1, UnitPrice = 100m };
        
        _context.Carts.Add(cart);
        _context.CartItems.Add(item);
        await _context.SaveChangesAsync();

        var result = await _controller.Remove(1);

        Assert.IsInstanceOfType<RedirectToActionResult>(result);
        Assert.AreEqual(0, _context.CartItems.Count());
    }
}
