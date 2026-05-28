using AlaiaStore.Domain.Entities;
using AlaiaStore.Domain.Interfaces;
using AlaiaStore.Web.ViewModels.Blog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AlaiaStore.Web.Services;

namespace AlaiaStore.Web.Controllers;

public class BlogController : Controller
{
    private readonly IRepository<BlogPost> _blogRepository;
    private readonly IImageService _imageService;

    public BlogController(IRepository<BlogPost> blogRepository, IImageService imageService)
    {
        _blogRepository = blogRepository;
        _imageService = imageService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var posts = await _blogRepository.GetAllAsync();
        var viewModel = posts
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new BlogPostListItemViewModel
            {
                Id = p.Id,
                Title = p.Title,
                ImageUrl = p.ImageUrl,
                Summary = p.Content.Length > 160 ? p.Content.Substring(0, 160) + "..." : p.Content,
                CreatedAt = p.CreatedAt
            });

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var post = await _blogRepository.GetByIdAsync(id);
        if (post == null || !post.IsPublished)
        {
            return NotFound();
        }

        var viewModel = new BlogPostDetailViewModel
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            CreatedAt = post.CreatedAt
        };

        return View(viewModel);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Admin()
    {
        var posts = await _blogRepository.GetAllAsync();
        var viewModel = posts
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new BlogPostListItemViewModel
            {
                Id = p.Id,
                Title = p.Title,
                ImageUrl = p.ImageUrl,
                Summary = p.Content.Length > 160 ? p.Content.Substring(0, 160) + "..." : p.Content,
                CreatedAt = p.CreatedAt
            });

        return View(viewModel);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new BlogPostFormViewModel());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(BlogPostFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.ImageFile != null)
        {
            var imageUrl = await _imageService.SaveImageAsync(model.ImageFile, "blog");
            if (!string.IsNullOrEmpty(imageUrl))
            {
                model.ImageUrl = imageUrl;
            }
        }

        var post = new BlogPost
        {
            Title = model.Title,
            Content = model.Content,
            ImageUrl = model.ImageUrl,
            IsPublished = model.IsPublished
        };

        await _blogRepository.AddAsync(post);
        return RedirectToAction(nameof(Admin));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _blogRepository.GetByIdAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        var model = new BlogPostFormViewModel
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            IsPublished = post.IsPublished
        };

        return View(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Edit(BlogPostFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var post = await _blogRepository.GetByIdAsync(model.Id);
        if (post == null)
        {
            return NotFound();
        }

        if (model.ImageFile != null)
        {
            var imageUrl = await _imageService.SaveImageAsync(model.ImageFile, "blog");
            if (!string.IsNullOrEmpty(imageUrl))
            {
                model.ImageUrl = imageUrl;
            }
        }

        post.Title = model.Title;
        post.Content = model.Content;
        
        if (!string.IsNullOrEmpty(model.ImageUrl))
        {
            post.ImageUrl = model.ImageUrl;
        }
        
        post.IsPublished = model.IsPublished;

        await _blogRepository.UpdateAsync(post);
        return RedirectToAction(nameof(Admin));
    }
}
