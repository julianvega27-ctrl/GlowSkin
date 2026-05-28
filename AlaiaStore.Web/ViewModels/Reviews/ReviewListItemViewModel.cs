namespace AlaiaStore.Web.ViewModels.Reviews;

public class ReviewListItemViewModel
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
