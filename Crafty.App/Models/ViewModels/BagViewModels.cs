using Crafty.App.Models.BindingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crafty.App.Models.ViewModels
{
  public class HomeViewModelBag
  {
    public IEnumerable<ConciseItemViewModel> PopularItems { get; set; }

    public IEnumerable<ConciseCategoryViewModel> PopularCategories { get; set; }

    public IEnumerable<ConciseBlogViewModel> Blogs { get; set; }
  }

  public class AddItemViewModelBag
  {
    public IEnumerable<ConciseSectionViewModel> Sections;

    public AddItemBindingModel AddItemBindingModel;
  }

  public class ItemDetailsViewModelBag
  {
    public ItemDetailsViewModel ItemDetailsViewModel;

    public IEnumerable<ConciseItemViewModel> SuggestedItems { get; set; }
  }

  public class PlaceOrderViewModelBag
  {
    public PlaceOrderBuyerViewModel BuyerViewModel { get; set; }

    public PlaceOrderSellerViewModel SellerViewModel { get; set; }

    public PlaceOrderItemViewModel ItemViewModel { get; set; }

    public PlaceOrderBindingModel OrderBindingModel { get; set; }
  }

  public class SearchViewModelBag
  {
    public IEnumerable<ConciseItemViewModel> ItemsByTitle { get; set; }

    public IEnumerable<ConciseItemViewModel> ItemsByDescription { get; set; }

    public string TargetWord { get; set; }
  }

  public class BlogDetailsViewModelBag
  {
    public BlogDetailsViewModel BlogModel { get; set; }

    public IEnumerable<ConciseBlogViewModel> OtherBlogs { get; set; }

    public bool MoreCommentsButton { get; set; }
  }
}