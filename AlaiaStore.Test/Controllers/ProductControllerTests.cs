using AlaiaStore.Domain.Entities;
using AlaiaStore.Domain.Interfaces;
using AlaiaStore.Web.Controllers;
using AlaiaStore.Web.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AlaiaStore.Test.Controllers;

[TestClass]
public class ProductControllerTests
{
    private Mock<IProductRepository> _mockProductRepo = null!;
    private Mock<ICategoryRepository> _mockCategoryRepo = null!;
    private Mock<IReviewRepository> _mockReviewRepo = null!;
    private Mock<IPromotionService> _mockPromotionService = null!;
    private Mock<IMapper> _mockMapper = null!;
    private ProductController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockProductRepo = new Mock<IProductRepository>();
        _mockCategoryRepo = new Mock<ICategoryRepository>();
        _mockReviewRepo = new Mock<IReviewRepository>();
        _mockPromotionService = new Mock<IPromotionService>();
        _mockMapper = new Mock<IMapper>();

        _controller = new ProductController(
            _mockProductRepo.Object,
            _mockCategoryRepo.Object,
            _mockReviewRepo.Object,
            _mockPromotionService.Object,
            _mockMapper.Object
        );
    }

    [TestMethod]
    public async Task Index_ReturnsView_WithProducts()
    {
        var products = new List<Product> { new Product { Id = 1, Name = "P1" } };
        _mockProductRepo.Setup(r => r.GetAllWithCategoryAsync()).ReturnsAsync(products);
        _mockCategoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Category>());

        var result = await _controller.Index(null);

        Assert.IsInstanceOfType<ViewResult>(result);
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult?.Model);
    }

    [TestMethod]
    public async Task Index_WithCategory_ReturnsFilteredProducts()
    {
        var products = new List<Product> { new Product { Id = 1, CategoryId = 2 } };
        _mockProductRepo.Setup(r => r.GetProductsByCategoryAsync(2)).ReturnsAsync(products);
        _mockCategoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Category>());

        var result = await _controller.Index(2);

        Assert.IsInstanceOfType<ViewResult>(result);
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult?.Model);
    }

    [TestMethod]
    public async Task Details_NullId_ReturnsNotFound()
    {
        // El metodo Details en el controlador recibe un int. Si no existe asume NotFound o BadRequest dependiendo de la logica.
        // Simularemos Id no encontrado
        _mockProductRepo.Setup(r => r.GetProductWithCategoryAsync(99)).ReturnsAsync((Product?)null);

        var result = await _controller.Details(99);

        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    [TestMethod]
    public async Task Details_ValidId_ReturnsViewWithProduct()
    {
        var product = new Product { Id = 1, Name = "P1" };
        _mockProductRepo.Setup(r => r.GetProductWithCategoryAsync(1)).ReturnsAsync(product);

        var result = await _controller.Details(1);

        Assert.IsInstanceOfType<ViewResult>(result);
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult?.Model);
    }

    [TestMethod]
    public async Task Search_ReturnsViewWithResults()
    {
        var products = new List<Product> { new Product { Id = 1, Name = "Test" } };
        _mockProductRepo.Setup(r => r.SearchProductsAsync("Test")).ReturnsAsync(products);

        var result = await _controller.Search("Test");

        Assert.IsInstanceOfType<ViewResult>(result);
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult?.Model);
    }
}
