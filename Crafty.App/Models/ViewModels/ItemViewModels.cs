using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crafty.App.Models.ViewModels
{
  public class ConciseItemViewModel
  {
    public int Id { get; set; }

    public string Title { get; set; }
    
    public string Thumbnail { get; set; }

    public decimal? Price { get; set; }

    //public string Seller { get; set; }

    //public string Location { get; set; }
  }

  public class BlogRelatedItemViewModel
  {
    public int Id { get; set; }

    public string Title { get; set; }

    public List<string> Pictures { get; set; }

    public decimal? Price { get; set; }
  }

  public class ItemDetailsViewModel
  {
    public int Id { get; set; }

    public string Title { get; set; }

    public string[] Pictures { get; set; }

    public string Description { get; set; }

    public string Category { get; set; }
    
    public int CategoryId { get; set; }

    public string Section { get; set; }

    public int SectionId { get; set; }

    public ConciseSellerViewModel Seller { get; set; }

    public string Location { get; set; }
    
    public decimal? Price { get; set; }   

    public int Likes { get; set; }

    public bool InFavourites { get; set; }

    public int Quantity { get; set; }

    public int Sales { get; set; }
  }

  public class MyItemViewModel
  {
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime PostedOn { get; set; }

    public string Thumbnail { get; set; }

    public decimal? Price { get; set; }
  }

  public class PlaceOrderItemViewModel
  {
    public int Id { get; set; }

    public string Thumbnail { get; set; }

    public string Description { get; set; }

    public decimal? Price { get; set; }

    public string Title { get; set; }
  }
}