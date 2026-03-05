using System.ComponentModel.DataAnnotations;

namespace New_project_2.ViewModels;

public class AdminAccessViewModel
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
