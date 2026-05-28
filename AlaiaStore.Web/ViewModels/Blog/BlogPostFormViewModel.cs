using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AlaiaStore.Web.ViewModels.Blog;

public class BlogPostFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(250)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public IFormFile? ImageFile { get; set; }

    public bool IsPublished { get; set; } = true;
}
