using System.ComponentModel.DataAnnotations;

namespace New_project_2.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Това поле е задължително.")]
    [MinLength(3,ErrorMessage = "{0} трябва да съдъжра поне {1} символа.")]
    [MaxLength(20, ErrorMessage = "{0} не може да бъде по-дълго от {1} символа.")]
    [Display(Name = "Името")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Това поле е задължително.")]
    [EmailAddress]
    [MaxLength(120)]
    [Display(Name = "Имейлът")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Това поле е задължително.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "{0} трябва да съдържа поне {1} симовила.")]
    [Display(Name = "Паролата")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Това поле е задължително.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Паролите не съвпадат.")]
    [Display(Name = "Повтори парола.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
