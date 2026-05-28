using AlaiaStore.Domain.Entities;
using AlaiaStore.Domain.Interfaces;
using AlaiaStore.Web.Controllers;
using AlaiaStore.Web.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlaiaStore.Test.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        private Mock<ILogger<HomeController>> _mockLogger = null!;
        private Mock<IProductRepository> _mockProductRepository = null!;
        private Mock<IMapper> _mockMapper = null!;
        private HomeController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockMapper = new Mock<IMapper>();

            _controller = new HomeController(
                _mockLogger.Object,
                _mockProductRepository.Object,
                _mockMapper.Object
            );
            
            // Contexto HTTP simulado necesario para el método Error
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [TestMethod]
        public async Task Index_ReturnsViewResult_WithFeaturedProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1" },
                new Product { Id = 2, Name = "Product 2" },
                new Product { Id = 3, Name = "Product 3" },
                new Product { Id = 4, Name = "Product 4" },
                new Product { Id = 5, Name = "Product 5" }
            };
            
            _mockProductRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(products);

            var productDtos = products.Take(4).Select(p => new ProductDto { Id = p.Id, Name = p.Name }).ToList();
            
            _mockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>()))
                .Returns(productDtos);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            
            var model = viewResult.Model as IEnumerable<ProductDto>;
            Assert.IsNotNull(model);
            Assert.AreEqual(4, model.Count());
        }

        [TestMethod]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
        }

        [TestMethod]
        public void Error_ReturnsViewResult_WithErrorViewModel()
        {
            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType<AlaiaStore.Web.Models.ErrorViewModel>(viewResult.Model);
        }
    }
}
