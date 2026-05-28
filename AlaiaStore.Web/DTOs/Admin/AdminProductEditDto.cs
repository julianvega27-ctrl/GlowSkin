using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AlaiaStore.Web.DTOs.Admin;

public class AdminProductEditDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(2000)]
    public string? Ingredients { get; set; }

    [StringLength(200)]
    public string? SkinType { get; set; }

    [Range(0.01, 999999)]
    public decimal Price { get; set; }

    [Range(0, 999999)]
    public int Stock { get; set; }

    [StringLength(500)]
    public string? MainImageUrl { get; set; }

    public IFormFile? ImageFile { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public bool IsActive { get; set; } = true;
}
