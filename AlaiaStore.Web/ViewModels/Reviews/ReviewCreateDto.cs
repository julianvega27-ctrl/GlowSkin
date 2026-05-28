using System.ComponentModel.DataAnnotations;

namespace AlaiaStore.Web.ViewModels.Reviews;

public class ReviewCreateDto
{
    public int ProductId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(1000)]
    public string? Comment { get; set; }
}
