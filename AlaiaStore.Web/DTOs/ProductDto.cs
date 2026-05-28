namespace AlaiaStore.Web.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Ingredients { get; set; }
    public string? SkinType { get; set; }
    public decimal Price { get; set; }
    public string? MainImageUrl { get; set; }
}
