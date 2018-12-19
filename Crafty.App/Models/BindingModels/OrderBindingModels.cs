
namespace Crafty.App.Models.BindingModels
{
  using System.ComponentModel.DataAnnotations;

  public class PlaceOrderBindingModel
  {
    [Required]
    public string BuyerId { get; set; }
  
    [Required]
    public string SellerId { get; set; }

    [Required]
    public int ItemId { get; set; }

    [Required]
    public int Quantity { get; set; }

    public string Details { get; set; }

    [Required(ErrorMessage = "Полето е задължително")]
    public string ShippingPhone { get; set; }

    [Required(ErrorMessage = "Посочете имена за доставка")]
    public string ShippingFullName { get; set; }

    [Required(ErrorMessage = "Полето е задължително")]
    public string ShippingAddress { get; set; }

    [Required(ErrorMessage = "Полето е задължително")]
    public string ShippingCity { get; set; }

    //[Required(ErrorMessage = "Посочете имейл за връзка")]
    //public string ShippingEmail { get; set; }
  }
}