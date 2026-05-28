using AlaiaStore.Domain.Entities;
using AlaiaStore.Domain.Interfaces;
using AlaiaStore.Web.ViewModels.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AlaiaStore.Web.Controllers;

[Authorize]
public class ReviewController : Controller
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;

    public ReviewController(IReviewRepository reviewRepository, IProductRepository productRepository, IUserRepository userRepository)
    {
        _reviewRepository = reviewRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReviewCreateDto model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Details", "Product", new { id = model.ProductId });
        }

        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdValue, out var userId))
        {
            return Forbid();
        }

        var product = await _productRepository.GetByIdAsync(model.ProductId);
        if (product == null)
        {
            return NotFound();
        }

        var review = new Review
        {
            ProductId = model.ProductId,
            UserId = userId,
            Rating = model.Rating,
            Comment = model.Comment
        };

        await _reviewRepository.AddAsync(review);
        return RedirectToAction("Details", "Product", new { id = model.ProductId });
    }
}
