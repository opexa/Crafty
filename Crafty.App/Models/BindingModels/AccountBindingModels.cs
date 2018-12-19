namespace Crafty.App.Models.BindingModels
{
  using System.ComponentModel.DataAnnotations;

  public class EditAccountBindingModel
  {
    [Display(Name = "Профилна снимка")]
    public string ProfileImg { get; set; }

    [Display(Name = "Собствено име")]
    [Required(ErrorMessage = "Имената sa задължителни")]
    public string FullName { get; set; }

    [Display(Name = "Телефон")]
    //[Required(ErrorMessage = "Въведете телефонен номер")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Въведете имейл адрес")]
    [Display(Name = "E-mail")]
    [DataType(DataType.EmailAddress, ErrorMessage = "Моля въведете валиден имейл адрес")]
    public string Email { get; set; }

    [Display(Name = "Описание")]
    public string Description { get; set; }

    [Display(Name = "Град")]
    public string City { get; set; }

    [Display(Name = "Адрес")]
    public string ShippingAddress { get; set; }

    //[Display(Name = "Статус")]
    //public string Status { get; set; }

    [Url(ErrorMessage = "Невалиден адрес")]
    [Display(Name = "Уебсайт")]
    public string Website { get; set; }

    [Display(Name = "Банер")]
    public string ProfileBanner { get; set; }
  }
}