namespace Crafty.App.App_Start
{
  using AutoMapper;
  using Crafty.Models;
  using Models.BindingModels;
  using Models.ViewModels;
  using System.Collections.Generic;
  using System.Linq;

  public class MapperConfig : Profile
  {
    public MapperConfig()
    {
      CreateMap<Blog, ConciseBlogViewModel>();

      CreateMap<Blog, BlogDetailsViewModel>()
        .ForMember(model => model.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count))
        .ForMember(model => model.Comments, opt => opt.MapFrom(src => Mapper.Map<IEnumerable<BlogCommentViewModel>>(src.Comments.OrderByDescending(c => c.PostedOn).Take(15))))
        .ForMember(model => model.RelatedItems, opt => opt.MapFrom(src => Mapper.Map<IEnumerable<BlogRelatedItemViewModel>>(src.RelatedItems)));

      CreateMap<Blog, EditBlogBindingModel>()
        .ForMember(model => model.CurrentRelatedItems, opt => opt.MapFrom(src => Mapper.Map<IEnumerable<ConciseItemViewModel>>(src.RelatedItems)));        

      CreateMap<Item, ConciseItemViewModel>();
        //.ForMember(model => model.Seller, opt => opt.MapFrom(src => src.Seller.UserName))
        //.ForMember(model => model.Location, opt => opt.MapFrom(src => src.Seller.City));

      CreateMap<Item, ItemDetailsViewModel>()
        .ForMember(model => model.Seller, opt => opt.MapFrom(src => src.Seller.UserName))
        .ForMember(model => model.Location, opt => opt.MapFrom(src => src.Seller.City))
        .ForMember(model => model.Pictures, opt => opt.MapFrom(src => src.Pictures.Split(';')))
        .ForMember(model => model.Category, opt => opt.MapFrom(src => src.Category.Name))
        .ForMember(model => model.Section, opt => opt.MapFrom(src => src.Category.Section.Name))
        .ForMember(model => model.Seller, opt => opt.MapFrom(src => Mapper.Map<ConciseSellerViewModel>(src.Seller)))
        .ForMember(model => model.Likes, opt => opt.MapFrom(src => src.Likes.Count))
        .ForMember(model => model.SectionId, opt => opt.MapFrom(src => src.Category.Section.Id))
        .ForMember(model => model.CategoryId, opt => opt.MapFrom(src => src.Category.Id));

      CreateMap<Item, MyItemViewModel>();

      CreateMap<Item, BlogRelatedItemViewModel>()
        .ForMember(model => model.Pictures, opt => opt.MapFrom(src => src.Pictures.Split(new[] { ';' }).Take(3).ToList()));

      CreateMap<Item, PlaceOrderItemViewModel>();

      CreateMap<Item, EditItemBindingModel>()
        .ForMember(bmodel => bmodel.CategoryId, opt => opt.MapFrom(src => src.Category.Id))
        .ForMember(bmodel => bmodel.OldPictures, opt => opt.MapFrom(src => src.Pictures.Split(new[] { ';' })));

      CreateMap<Section, DetailSectionViewModel>()
        .ForMember(model => model.Categories, opt => opt.MapFrom(src => Mapper.Map<IEnumerable<MenuCategoryViewModel>>(src.Categories)));

      CreateMap<Section, ConciseSectionViewModel>();
      CreateMap<Section, MenuSectionViewModel>()
        .ForMember(model => model.Categories, opt => opt.MapFrom(src => Mapper.Map<IEnumerable<MenuCategoryViewModel>>(src.Categories)));

      CreateMap<Category, ConciseCategoryViewModel>()
        .ForMember(model => model.SectionId, opt => opt.MapFrom(src => src.Section.Id));
      CreateMap<Category, MenuCategoryViewModel>();

      CreateMap<User, ProfileDetailsViewModel>()
        .ForMember(model => model.Status, opt => opt.MapFrom(src => src.Status.ToString()));

      CreateMap<User, UserProfileDetailsViewModel>()
       .ForMember(model => model.Status, opt => opt.MapFrom(src => src.Status.ToString()))
       .ForMember(model => model.Items, opt => opt.MapFrom(src => Mapper.Map<IEnumerable<ConciseItemViewModel>>(src.ItemsForSale.OrderByDescending(i => i.PostedOn).Take(25))));

      CreateMap<User, EditAccountBindingModel>();
        //.ForMember(model => model.Status, opt => opt.MapFrom(src => src.Status.ToString()));

      CreateMap<User, ConciseSellerViewModel>();

      CreateMap<User, PlaceOrderSellerViewModel>();
      CreateMap<User, PlaceOrderBuyerViewModel>();

      CreateMap<Order, ConcisePurchaseViewModel>()
        .ForMember(model => model.ItemId, opt => opt.MapFrom(src => src.Item.Id))
        .ForMember(model => model.ItemTitle, opt => opt.MapFrom(src => src.Item.Title))
        .ForMember(model => model.ItemThumbnail, opt => opt.MapFrom(src => src.Item.Thumbnail))
        .ForMember(model => model.ItemPrice, opt => opt.MapFrom(src => src.Item.Price))
        .ForMember(model => model.SellerUserName, opt => opt.MapFrom(src => src.Seller.UserName))
        .ForMember(model => model.SellerId, opt => opt.MapFrom(src => src.Seller.Id))
        .ForMember(model => model.OrderTrackingNum, opt => opt.MapFrom(src => src.TrackingNumber))
        .ForMember(model => model.OrderShippedOn, opt => opt.MapFrom(src => src.ShippedOn))
        .ForMember(model => model.OrderStatus, opt => opt.MapFrom(src => src.Status.ToString()));

      CreateMap<Order, ConciseOrderViewModel>()
        .ForMember(model => model.ItemId, opt => opt.MapFrom(src => src.Item.Id))
        .ForMember(model => model.ItemTitle, opt => opt.MapFrom(src => src.Item.Title))
        .ForMember(model => model.ItemThumbnail, opt => opt.MapFrom(src => src.Item.Thumbnail))
        .ForMember(model => model.OrderId, opt => opt.MapFrom(src => src.Id))
        .ForMember(model => model.OrderBuyerUsername, opt => opt.MapFrom(src => src.Buyer.UserName))
        .ForMember(model => model.OrderBuyerId, opt => opt.MapFrom(src => src.Buyer.Id))
        .ForMember(model => model.OrderShippingFullName, opt => opt.MapFrom(src => src.ShippingFullName))
        .ForMember(model => model.OrderShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
        .ForMember(model => model.OrderShippingCity, opt => opt.MapFrom(src => src.ShippingCity))
        .ForMember(model => model.OrderDetails, opt => opt.MapFrom(src => src.Details))
        .ForMember(model => model.OrderAmount, opt => opt.MapFrom(src => src.Amount))
        .ForMember(model => model.OrderPostedOn, opt => opt.MapFrom(src => src.PostedOn))
        .ForMember(model => model.ItemPostedOn, opt => opt.MapFrom(src => src.Item.PostedOn))
        .ForMember(model => model.ItemPrice, opt => opt.MapFrom(src => src.Item.Price))
        .ForMember(model => model.OrderTrackingNum, opt => opt.MapFrom(src => src.TrackingNumber))
        .ForMember(model => model.OrderShippedOn, opt => opt.MapFrom(src => src.ShippedOn));

      CreateMap<Order, ConciseFinOrderViewModel>()
        .ForMember(model => model.ItemId, opt => opt.MapFrom(src => src.Item.Id))
        .ForMember(model => model.ItemTitle, opt => opt.MapFrom(src => src.Item.Title))
        .ForMember(model => model.OrderId, opt => opt.MapFrom(src => src.Id))
        .ForMember(model => model.OrderBuyerUsername, opt => opt.MapFrom(src => src.Buyer.UserName))
        .ForMember(model => model.OrderBuyerId, opt => opt.MapFrom(src => src.Buyer.Id))
        .ForMember(model => model.OrderAmount, opt => opt.MapFrom(src => src.Amount))
        .ForMember(model => model.OrderPostedOn, opt => opt.MapFrom(src => src.PostedOn))
        .ForMember(model => model.ItemPrice, opt => opt.MapFrom(src => src.Item.Price))
        .ForMember(model => model.OrderStatus, opt => opt.MapFrom(src => src.Status.ToString()));

      CreateMap<Order, OrderDetailsViewModel>()
        .ForMember(model => model.SellerId, opt => opt.MapFrom(src => src.Seller.Id))
        .ForMember(model => model.SellerUsername, opt => opt.MapFrom(src => src.Seller.UserName))
        .ForMember(model => model.ItemId, opt => opt.MapFrom(src => src.Item.Id))
        .ForMember(model => model.ItemTitle, opt => opt.MapFrom(src => src.Item.Title))
        .ForMember(model => model.ItemThumbnail, opt => opt.MapFrom(src => src.Item.Thumbnail))
        .ForMember(model => model.OrderId, opt => opt.MapFrom(src => src.Id))
        .ForMember(model => model.OrderBuyerUsername, opt => opt.MapFrom(src => src.Buyer.UserName))
        .ForMember(model => model.OrderBuyerId, opt => opt.MapFrom(src => src.Buyer.Id))
        .ForMember(model => model.OrderShippingFullName, opt => opt.MapFrom(src => src.ShippingFullName))
        .ForMember(model => model.OrderShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
        .ForMember(model => model.OrderShippingCity, opt => opt.MapFrom(src => src.ShippingCity))
        .ForMember(model => model.OrderDetails, opt => opt.MapFrom(src => src.Details))
        .ForMember(model => model.OrderAmount, opt => opt.MapFrom(src => src.Amount))
        .ForMember(model => model.OrderPostedOn, opt => opt.MapFrom(src => src.PostedOn))
        .ForMember(model => model.ItemPostedOn, opt => opt.MapFrom(src => src.Item.PostedOn))
        .ForMember(model => model.ItemPrice, opt => opt.MapFrom(src => src.Item.Price))
        .ForMember(model => model.OrderTrackingNum, opt => opt.MapFrom(src => src.TrackingNumber))
        .ForMember(model => model.OrderShippedOn, opt => opt.MapFrom(src => src.ShippedOn))
        .ForMember(model => model.BuyerFeedback, opt => opt.MapFrom(src => src.BuyerFeedback.Message))
        .ForMember(model => model.BuyerGivenStars, opt => opt.MapFrom(src => src.BuyerFeedback.Stars))
        .ForMember(model => model.SellerFeedback, opt => opt.MapFrom(src => src.SellerFeedback.Message))
        .ForMember(model => model.SellerGivenStars, opt => opt.MapFrom(src => src.SellerFeedback.Stars))
        .ForMember(model => model.OrderStatus, opt => opt.MapFrom(src => src.Status.ToString()));

      CreateMap<Notification, ConciseNotificationViewModel>()
         .ForMember(model => model.Type, opt => opt.MapFrom(src => src.Type.ToString()))
         .ForMember(model => model.SenderName, opt => opt.MapFrom(src => src.Sender.UserName));


      CreateMap<BlogComment, BlogCommentViewModel>()
        .ForMember(model => model.UserName, opt => opt.MapFrom(src => src.Author.UserName))
        .ForMember(model => model.PostedOn, opt => opt.MapFrom(src => src.PostedOn.ToRelativeDateString()))
        .ForMember(model => model.UserPicture, opt => opt.MapFrom(src => src.Author.ProfileImg));
    }
  }
}