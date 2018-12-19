namespace Crafty.App.Models.ViewModels
{
  using Crafty.Models;
  using System.Collections.Generic;

  public class ProfileDetailsViewModel
  {
    public string UserName { get; set; }

    public string City { get; set; }

    public string ProfileBanner { get; set; }

    public string ProfileImg { get; set; }

    public string Status { get; set; }

    public string Website { get; set; }

    public string Description { get; set; }

    public string Email { get; set; }

    public int? SoldItems { get; set; }
  }

  public class UserProfileDetailsViewModel
  {
    public string UserName { get; set; }

    public string City { get; set; }

    public string ProfileBanner { get; set; }

    public string ProfileImg { get; set; }

    public string Status { get; set; }

    public string Website { get; set; }

    public string Description { get; set; }

    public string Email { get; set; }

    public int? SoldItems { get; set; }

    public IEnumerable<ConciseItemViewModel> Items { get; set; }
  }

  public class ConciseSellerViewModel
  {
    public string UserName { get; set; }

    public string Description { get; set; }

    public string ProfileImg { get; set; }

    public string City { get; set; }
  }

  public class PlaceOrderSellerViewModel
  {
    public string UserName { get; set; }

    public string City { get; set; }

    public string ProfileImg { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }
  }

  public class PlaceOrderBuyerViewModel
  {
    public string UserName { get; set; }

    public string City { get; set; }

    public string ShippingAddress { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }
  }
}