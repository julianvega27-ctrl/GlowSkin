using AlaiaStore.Domain.Entities;
using AlaiaStore.Domain.Interfaces;
using AlaiaStore.Web.DTOs.Admin;
using AlaiaStore.Web.ViewModels;
using AlaiaStore.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AlaiaStore.Web.Services;

namespace AlaiaStore.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IImageService _imageService;

    public AdminController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IOrderRepository orderRepository,
        IImageService imageService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _imageService = imageService;
    }

    public async Task<IActionResult> Index()
    {
        var productsTask = _productRepository.GetAllAsync();
        var categoriesTask = _categoryRepository.GetAllAsync();
        var usersTask = _userRepository.GetAllAsync();
        var ordersTask = _orderRepository.GetAllAsync();

        await Task.WhenAll(productsTask, categoriesTask, usersTask, ordersTask);

        var totalSales = ordersTask.Result.Sum(o => o.TotalAmount);

        var viewModel = new AdminDashboardViewModel
        {
            TotalProducts = productsTask.Result.Count(),
            TotalCategories = categoriesTask.Result.Count(),
            TotalUsers = usersTask.Result.Count(),
            TotalOrders = ordersTask.Result.Count(),
            TotalSales = totalSales
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Products(int? categoryId)
    {
        var categories = await _categoryRepository.GetAllAsync();
        var products = categoryId.HasValue
            ? await _productRepository.GetProductsByCategoryAsync(categoryId.Value)
            : await _productRepository.GetAllWithCategoryAsync();

        var viewModel = new AdminProductListViewModel
        {
            Categories = categories.Select(c => new DTOs.CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl
            }),
            Products = products.Select(p => new AdminProductListItemViewModel
            {
                Id = p.Id,
                Name = p.Name,
                CategoryName = p.Category?.Name ?? string.Empty,
                Price = p.Price,
                Stock = p.Stock,
                IsActive = p.IsActive
            }),
            SelectedCategoryId = categoryId
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> CreateProduct()
    {
        var categories = await _categoryRepository.GetAllAsync();
        var viewModel = new AdminProductFormViewModel
        {
            Categories = categories.Select(c => new DTOs.CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl
            })
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([Bind(Prefix = "Product")] AdminProductEditDto model)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(new AdminProductFormViewModel
            {
                Product = model,
                Categories = categories.Select(c => new DTOs.CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl
                })
            });
        }

        if (model.ImageFile != null)
        {
            var imageUrl = await _imageService.SaveImageAsync(model.ImageFile, "products");
            if (!string.IsNullOrEmpty(imageUrl))
            {
                model.MainImageUrl = imageUrl;
            }
        }

        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            Ingredients = model.Ingredients,
            SkinType = model.SkinType,
            Price = model.Price,
            Stock = model.Stock,
            MainImageUrl = model.MainImageUrl,
            CategoryId = model.CategoryId,
            IsActive = model.IsActive,
            IsBestSeller = model.IsBestSeller
        };

        await _productRepository.AddAsync(product);
        return RedirectToAction(nameof(Products));
    }

    [HttpGet]
    public async Task<IActionResult> EditProduct(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var categories = await _categoryRepository.GetAllAsync();
        var viewModel = new AdminProductFormViewModel
        {
            Product = new AdminProductEditDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Ingredients = product.Ingredients,
                SkinType = product.SkinType,
                Price = product.Price,
                Stock = product.Stock,
                MainImageUrl = product.MainImageUrl,
                CategoryId = product.CategoryId,
                IsActive = product.IsActive,
                IsBestSeller = product.IsBestSeller
            },
            Categories = categories.Select(c => new DTOs.CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl
            })
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditProduct([Bind(Prefix = "Product")] AdminProductEditDto model)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(new AdminProductFormViewModel
            {
                Product = model,
                Categories = categories.Select(c => new DTOs.CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl
                })
            });
        }

        var product = await _productRepository.GetByIdAsync(model.Id);
        if (product == null)
        {
            return NotFound();
        }

        if (model.ImageFile != null)
        {
            var imageUrl = await _imageService.SaveImageAsync(model.ImageFile, "products");
            if (!string.IsNullOrEmpty(imageUrl))
            {
                model.MainImageUrl = imageUrl;
            }
        }

        product.Name = model.Name;
        product.Description = model.Description;
        product.Ingredients = model.Ingredients;
        product.SkinType = model.SkinType;
        product.Price = model.Price;
        product.Stock = model.Stock;
        
        if (!string.IsNullOrEmpty(model.MainImageUrl))
        {
            product.MainImageUrl = model.MainImageUrl;
        }
        
        product.CategoryId = model.CategoryId;
        product.IsActive = model.IsActive;
        product.IsBestSeller = model.IsBestSeller;

        await _productRepository.UpdateAsync(product);
        return RedirectToAction(nameof(Products));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleProductStatus(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        product.IsActive = !product.IsActive;
        await _productRepository.UpdateAsync(product);
        return RedirectToAction(nameof(Products));
    }

    public async Task<IActionResult> Orders()
    {
        var orders = await _orderRepository.GetAllAsync();
        var viewModel = new AdminOrderListViewModel
        {
            Orders = orders.Select(o => new AdminOrderListItemViewModel
            {
                Id = o.Id,
                CustomerName = o.User != null ? $"{o.User.FirstName} {o.User.LastName}" : "",
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentStatus = o.PaymentStatus,
                CreatedAt = o.CreatedAt
            })
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Users()
    {
        var users = await _userRepository.GetAllAsync();
        var viewModel = new AdminUserListViewModel
        {
            Users = users.Select(u => new AdminUserListItemViewModel
            {
                Id = u.Id,
                FullName = $"{u.FirstName} {u.LastName}",
                Email = u.Email,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            })
        };

        return View(viewModel);
    }
}
