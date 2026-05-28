using AlaiaStore.Domain.Entities;
using AlaiaStore.Domain.Interfaces;
using AlaiaStore.Web.ViewModels.Promotions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlaiaStore.Web.Controllers;

[Authorize(Roles = "Admin")]
public class PromotionController : Controller
{
    private readonly IRepository<Promotion> _promotionRepository;

    public PromotionController(IRepository<Promotion> promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    public async Task<IActionResult> Index()
    {
        var promotions = await _promotionRepository.GetAllAsync();
        var viewModel = promotions
            .OrderByDescending(p => p.StartDate)
            .Select(p => new PromotionListItemViewModel
            {
                Id = p.Id,
                Title = p.Title,
                DiscountPercentage = p.DiscountPercentage,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                IsActive = p.IsActive
            });

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new PromotionFormViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(PromotionFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var promotion = new Promotion
        {
            Title = model.Title,
            Description = model.Description,
            DiscountPercentage = model.DiscountPercentage,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            IsActive = model.IsActive
        };

        await _promotionRepository.AddAsync(promotion);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var promotion = await _promotionRepository.GetByIdAsync(id);
        if (promotion == null)
        {
            return NotFound();
        }

        var model = new PromotionFormViewModel
        {
            Id = promotion.Id,
            Title = promotion.Title,
            Description = promotion.Description,
            DiscountPercentage = promotion.DiscountPercentage,
            StartDate = promotion.StartDate,
            EndDate = promotion.EndDate,
            IsActive = promotion.IsActive
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(PromotionFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var promotion = await _promotionRepository.GetByIdAsync(model.Id);
        if (promotion == null)
        {
            return NotFound();
        }

        promotion.Title = model.Title;
        promotion.Description = model.Description;
        promotion.DiscountPercentage = model.DiscountPercentage;
        promotion.StartDate = model.StartDate;
        promotion.EndDate = model.EndDate;
        promotion.IsActive = model.IsActive;

        await _promotionRepository.UpdateAsync(promotion);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var promotion = await _promotionRepository.GetByIdAsync(id);
        if (promotion == null)
        {
            return NotFound();
        }

        promotion.IsActive = !promotion.IsActive;
        await _promotionRepository.UpdateAsync(promotion);
        return RedirectToAction(nameof(Index));
    }
}
