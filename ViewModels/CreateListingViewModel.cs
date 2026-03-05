using System.ComponentModel.DataAnnotations;

namespace New_project_2.ViewModels;

public class CreateListingViewModel
{
    [Required]
    [MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 1000000000)]
    public decimal Price { get; set; }

    [Required]
    [MaxLength(60)]
    public string Category { get; set; } = string.Empty;

    [Display(Name = "Image URL")]
    [MaxLength(600)]
    public string ImageUrl { get; set; } = string.Empty;
}
