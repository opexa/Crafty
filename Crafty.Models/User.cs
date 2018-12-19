namespace Crafty.Models
{
  using Microsoft.AspNet.Identity;
  using Microsoft.AspNet.Identity.EntityFramework;
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.Security.Claims;
  using System.Threading.Tasks;

  public class User : IdentityUser
  {
    public User()
    {
      this.ItemsForSale = new HashSet<Item>();
      this.Likes = new HashSet<Like>();
      this.Favourites = new HashSet<Item>();
      this.AwaitingOrders = new HashSet<Order>();
      this.SentOrders = new HashSet<Order>();
      this.FinishedOrders = new HashSet<Order>();
      this.PurchaseHistory = new HashSet<Order>();
      this.GivenFeedbacks = new HashSet<Feedback>();
      this.ReceivedFeedbacks = new HashSet<Feedback>();
      this.Notifications = new HashSet<Notification>();
    }
    
    public string ProfileImg { get; set; }
    
    public string Description { get; set; }

    [Required]
    public string FullName { get; set; }

    public string City { get; set; } 

    public int? SoldItems { get; set; }

    [Required]
    public UserStatusType Status { get; set; }

    [DataType(DataType.Url)]
    public string Website { get; set; }

    public string ProfileBanner { get; set; }

    public string ShippingAddress { get; set; }

    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
    {
      var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
      return userIdentity;
    }

    public virtual ICollection<Item> ItemsForSale { get; set; }

    public virtual ICollection<Like> Likes { get; set; }

    public virtual ICollection<Item> Favourites { get; set; }

    public virtual ICollection<Order> AwaitingOrders { get; set; }

    public virtual ICollection<Order> SentOrders { get; set; }

    public virtual ICollection<Order> FinishedOrders { get; set; }

    public virtual ICollection<Order> PurchaseHistory { get; set; }

    public virtual ICollection<Feedback> GivenFeedbacks { get; set; }

    public virtual ICollection<Feedback> ReceivedFeedbacks { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; }
  }
}
