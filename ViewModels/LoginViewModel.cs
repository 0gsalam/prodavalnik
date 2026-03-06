using System.ComponentModel.DataAnnotations;

namespace New_project_2.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Това поле е задължително.")]
    [Display(Name = "Име или имейл")]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Това поле е задължително.")]
    [DataType(DataType.Password)]
    [Display(Name = "Парола")]
    public string Password { get; set; } = string.Empty;
}
