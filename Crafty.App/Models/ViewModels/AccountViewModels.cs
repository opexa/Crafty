using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Crafty.App.Models.ViewModels
{
  public class ExternalLoginConfirmationViewModel
  {
    [Required(ErrorMessage = "Моля въведете потребителско име.")]
    [Display(Name = "Потребителско име")]
    public string Username { get; set; }
  }

  public class ExternalLoginListViewModel
  {
    public string ReturnUrl { get; set; }
  }

  public class SendCodeViewModel
  {
    public string SelectedProvider { get; set; }
    public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    public string ReturnUrl { get; set; }
    public bool RememberMe { get; set; }
  }

  public class VerifyCodeViewModel
  {
    [Required]
    public string Provider { get; set; }

    [Required]
    [Display(Name = "Code")]
    public string Code { get; set; }
    public string ReturnUrl { get; set; }

    [Display(Name = "Remember this browser?")]
    public bool RememberBrowser { get; set; }

    public bool RememberMe { get; set; }
  }

  public class ForgotViewModel
  {
    [Required]
    [Display(Name = "Email")]
    public string Email { get; set; }
  }

  public class LoginViewModel
  {
    [Required(ErrorMessage = "Въведете потребителско име")]
    [Display(Name = "Потребителско име")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Въведете парола")]
    [DataType(DataType.Password)]
    [Display(Name = "Парола")]
    public string Password { get; set; }

    [Display(Name = "Запомни ме")]
    public bool RememberMe { get; set; }
  }

  public class RegisterViewModel
  {
    [Required(ErrorMessage = "Потребителското име е задължително")]
    [Display(Name = "Потребителско име")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Моля въведете имена")]
    [Display(Name = "Вашите имена")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Имейлът е задължителен")]
    [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    //[Required(ErrorMessage = "Моля посочете статусът, който ще заемете в нашия сайт.")]
    //[Display(Name = "Статус")]
    //public string Status { get; set; }

    //[Required(ErrorMessage = "Телефонният номер е задължителен")]
    //[Display(Name = "Телефон")]
    //[DataType(DataType.PhoneNumber)]
    //public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Моля въведете парола")]
    [StringLength(100, ErrorMessage = "Паролата трябва да бъде поне {2} символа дълга.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Парола")]
    public string Password { get; set; }

    //[DataType(DataType.Password)]
    //[Display(Name = "Потвърди парола")]
    //[Compare("Password", ErrorMessage = "Паролите не съвпадат.")]
    //public string ConfirmPassword { get; set; }
  }

  public class ResetPasswordViewModel
  {
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    public string Code { get; set; }
  }

  public class ForgotPasswordViewModel
  {
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }
  }
}
