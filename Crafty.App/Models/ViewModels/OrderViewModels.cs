using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crafty.App.Models.ViewModels
{
  public class ConcisePurchaseViewModel
  {
    public int Id { get; set; }

    public string ItemId { get; set; }

    public string ItemTitle { get; set; }

    public string ItemThumbnail { get; set; }

    public string SellerUserName { get; set; }

    public string OrderTrackingNum { get; set; }

    public DateTime OrderShippedOn { get; set; }
    
    public string SellerId { get; set; }

    public string OrderStatus { get; set; }

    public decimal? ItemPrice { get; set; }

    public int Amount { get; set; }

    public DateTime PostedOn { get; set; }
  }

  public class ConciseOrderViewModel
  {
    public int Id { get; set; }

    public int ItemId { get; set; }

    public DateTime ItemPostedOn { get; set; }

    public string ItemTitle { get; set; }

    public decimal? ItemPrice { get; set; }

    public string ItemThumbnail { get; set; }

    public int OrderId { get; set; }

    public string OrderBuyerUsername { get; set; }

    public string OrderBuyerId { get; set; }

    public string OrderShippingFullName { get; set; }

    public string OrderShippingCity { get; set; }

    public string OrderShippingAddress { get; set; }

    public string OrderDetails { get; set; }

    public int OrderAmount { get; set; }

    public DateTime OrderPostedOn { get; set; }

    public string OrderTrackingNum { get; set; }

    public DateTime? OrderShippedOn { get; set; }
  }

  public class ConciseFinOrderViewModel
  {
    public int Id { get; set; }

    public int ItemId { get; set; }

    public string ItemTitle { get; set; }

    public decimal? ItemPrice { get; set; }

    public int OrderId { get; set; }

    public string OrderBuyerUsername { get; set; }

    public string OrderBuyerId { get; set; }

    public int OrderAmount { get; set; }

    public DateTime OrderPostedOn { get; set; }

    public string OrderStatus { get; set; }
  }

  public class OrderDetailsViewModel
  {
    public string SellerId { get; set; }

    public string SellerUsername { get; set; }

    public int ItemId { get; set; }

    public DateTime ItemPostedOn { get; set; }

    public string ItemTitle { get; set; }

    public decimal? ItemPrice { get; set; }

    public string ItemThumbnail { get; set; }

    public int OrderId { get; set; }

    public string OrderBuyerUsername { get; set; }

    public string OrderBuyerId { get; set; }

    public string OrderShippingFullName { get; set; }

    public string OrderShippingCity { get; set; }

    public string OrderShippingAddress { get; set; }

    public string OrderDetails { get; set; }

    public int OrderAmount { get; set; }

    public DateTime OrderPostedOn { get; set; }

    public string OrderTrackingNum { get; set; }

    public DateTime? OrderShippedOn { get; set; }

    public string BuyerFeedback { get; set; }

    public int BuyerGivenStars { get; set; }

    public string SellerFeedback { get; set; }

    public int SellerGivenStars { get; set; }

    public string OrderStatus { get; set; }
  }
}