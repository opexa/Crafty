namespace Crafty.Models
{
  using System;
  using System.ComponentModel;
  using System.ComponentModel.DataAnnotations;

  public class Order
  {
    [Key]
    public int Id { get; set; }

    public virtual User Seller { get; set; }

    [Required]
    public string SellerId { get; set; }

    public virtual Item Item { get; set; }

    [Required]
    public int ItemId { get; set; }

    public virtual User Buyer { get; set; }

    [Required]
    public string BuyerId { get; set; }

    public virtual Feedback BuyerFeedback { get; set; }

    public virtual Feedback SellerFeedback { get; set; }
    
    public int FeedbackId { get; set; }

    [Required]
    public DateTime PostedOn { get; set; }

    [Range(1, 9999999999)]
    public int Amount { get; set; }

    [Required]
    public string ShippingFullName { get; set; }

    [Required]
    public string ShippingPhone { get; set; }

    [Required]
    public string ShippingAddress { get; set; }

    [Required]
    public string ShippingCity { get; set; }

    [MaxLength(1000)]
    public string Details { get; set; }

    [Required]
    [DefaultValue(OrderStatus.Awaiting)]
    public OrderStatus Status { get; set; }

    public DateTime? ShippedOn { get; set; }

    public string TrackingNumber { get; set; }

  }

}
