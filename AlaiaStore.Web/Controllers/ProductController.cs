using AlaiaStore.Domain.Interfaces;
using AlaiaStore.Web.DTOs;
using AlaiaStore.Web.ViewModels;
using AlaiaStore.Web.ViewModels.Reviews;
using AlaiaStore.Web.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AlaiaStore.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IReviewRepository _reviewRepository;
    private readonly IPromotionService _promotionService;

    public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, IReviewRepository reviewRepository, IPromotionService promotionService, IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _reviewRepository = reviewRepository;
        _promotionService = promotionService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(int? categoryId)
    {
        var categories = await _categoryRepository.GetAllAsync();
        var products = categoryId.HasValue 
            ? await _productRepository.GetProductsByCategoryAsync(categoryId.Value) 
            : await _productRepository.GetAllAsync();

        var viewModel = new ProductCatalogViewModel
        {
            Categories = _mapper.Map<IEnumerable<CategoryDto>>(categories),
            Products = _mapper.Map<IEnumerable<ProductDto>>(products),
            SelectedCategoryId = categoryId
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productRepository.GetProductWithCategoryAsync(id);
        if (product == null)
            return NotFound();

        var productDto = _mapper.Map<ProductDto>(product);
        var discountPercentage = await _promotionService.GetActiveDiscountPercentageAsync(product.Id, DateTime.UtcNow);
        var reviews = await _reviewRepository.GetReviewsByProductIdAsync(id);

        var viewModel = new ProductReviewsViewModel
        {
            Product = productDto,
            Reviews = reviews.Select(r => new ReviewListItemViewModel
            {
                Rating = r.Rating,
                Comment = r.Comment,
                UserName = r.User != null ? $"{r.User.FirstName} {r.User.LastName}" : "",
                CreatedAt = r.CreatedAt
            }),
            NewReview = new ReviewCreateDto { ProductId = id }
        };

        ViewData["DiscountPercentage"] = discountPercentage;

        return View(viewModel);
    }

    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return RedirectToAction(nameof(Index));
        }

        var products = await _productRepository.SearchProductsAsync(query);
        var viewModel = new ProductCatalogViewModel
        {
            Products = _mapper.Map<IEnumerable<ProductDto>>(products),
            Categories = new List<CategoryDto>(),
            SelectedCategoryId = null
        };

        ViewData["SearchQuery"] = query;
        return View("Search", viewModel);
    }
}