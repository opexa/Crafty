namespace Crafty.App.Controllers
{
  using AutoMapper;
  using Crafty.Models;
  using Data;
  using Data.UnitOfWork;
  using Hubs;
  using Microsoft.AspNet.Identity;
  using Microsoft.AspNet.SignalR;
  using Models.BindingModels;
  using Models.ViewModels;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using System.Web.Mvc;
  using System.Web.Security;

  [System.Web.Mvc.Authorize]
  public class OrdersController : BaseController
  {
    public OrdersController(ICraftyData data) : base(data) { }

    [HttpGet]
    public ActionResult Place(string i, string q)
    {
      Item item = this.Data.Items.Find(int.Parse(i));
      if (item == null)
        return RedirectToAction("", "error", null);

      if(int.Parse(q) < 1)
      {
        this.RedirectToAction("details", "item", new { Id = int.Parse(i) });
      }

      //TODO: Implement check if already there is no availability of that product to show a message and redirect.

      PlaceOrderViewModelBag model = new PlaceOrderViewModelBag
      {
        BuyerViewModel = Mapper.Map<PlaceOrderBuyerViewModel>(this.UserProfile),
        SellerViewModel = Mapper.Map<PlaceOrderSellerViewModel>(item.Seller),
        ItemViewModel = Mapper.Map<PlaceOrderItemViewModel>(item),
        OrderBindingModel = new PlaceOrderBindingModel()
        {
          BuyerId = this.User.Identity.GetUserId(),
          SellerId = item.Seller.Id,
          ItemId = item.Id,
          Quantity = int.Parse(q),
          ShippingAddress = this.UserProfile.ShippingAddress,
          ShippingCity = this.UserProfile.City,
          ShippingFullName = this.UserProfile.FullName,
          ShippingPhone = this.UserProfile.PhoneNumber
          //ShippingEmail = this.UserProfile.Email
        }
      };

      return this.View(model);
    }

    [HttpPost]
    public ActionResult Place(PlaceOrderBindingModel model)
    {
      if(this.ModelState.IsValid)
      {
        User seller = this.Data.Users.Find(model.SellerId);
        Item item = this.Data.Items.Find(model.ItemId);
        Order order = new Order()
        {
          Buyer = this.UserProfile,
          Seller = seller,
          Amount = model.Quantity,
          Item = item,
          PostedOn = DateTime.Now,
          ShippingAddress = model.ShippingAddress,
          ShippingCity = model.ShippingCity,
          ShippingFullName = model.ShippingFullName,
          ShippingPhone = model.ShippingPhone,
          Status = OrderStatus.Awaiting,
          Details = model.Details
        };

        User admin = this.Data.Users.All().FirstOrDefault(u => u.UserName == "admin");

        try
        {
          admin.AwaitingOrders.Add(order);
          UserProfile.PurchaseHistory.Add(order);
          item.Quantity -= model.Quantity;

          Notification notification = new Notification()
          {
            Sender = this.UserProfile,
            Receiver = admin,
            Type = NotificationType.NewOrder,
            ObjectId = order.Item.Id,
            ObjectName = order.Item.Title,
            PostedOn = DateTime.Now
            //ObjectType = NotificationObejctType.Item
          };
          admin.Notifications.Add(notification);

          this.Data.SaveChanges();

          this.SendClientNotification(order.Seller.Id, notification.Id);

          return RedirectToAction("details", "items", new { Id = model.ItemId });
        }
        catch(Exception ex)
        {
          this.ViewBag.Exception = "Възникна грешка при потвърждаването на вашата поръчка.Моля опитайте отново и ако възникне проблем отново, се свържете с нас.";
        }
      }
      return this.RedirectToAction("place", new { i = model.ItemId, q = model.Quantity });
    }

    [HttpGet]
    [System.Web.Mvc.Authorize(Roles = "Admin")]
    public ActionResult Awaiting()
    {
      IEnumerable<ConciseOrderViewModel> model = Mapper.Map<IEnumerable<ConciseOrderViewModel>>(this.UserProfile.AwaitingOrders);
      this.ViewBag.aoCount = this.UserProfile.AwaitingOrders.Count;
      return View(model);
    }

    [HttpGet]
    [System.Web.Mvc.Authorize(Roles = "Admin")]
    public ActionResult Sent()
    {
      IEnumerable<ConciseOrderViewModel> model = Mapper.Map<IEnumerable<ConciseOrderViewModel>>(this.UserProfile.SentOrders);
      this.ViewBag.aoCount = this.UserProfile.AwaitingOrders.Count;
      return View(model);
    }

    [HttpPost]
    [System.Web.Mvc.Authorize(Roles = "Admin")]
    public ActionResult StatSent(string o, string tn)
    {
      int orderId = int.Parse(o);
      Order order = this.UserProfile.AwaitingOrders.FirstOrDefault(or => or.Id == orderId);

      if (order != null)
      {
        try
        {
          this.UserProfile.SentOrders.Add(order);
          this.UserProfile.AwaitingOrders.Remove(order);
          order.Status = OrderStatus.Sent;
          order.TrackingNumber = tn;
          order.ShippedOn = DateTime.Now;

          Notification notification = this.CreateOrderNotification(order.Id, order.Item.Title, order.Buyer, NotificationType.SentOrder);
          order.Buyer.Notifications.Add(notification);
          this.Data.SaveChanges();

          var view = PartialView("ConciseNotificationViewModel", notification);

          this.SendClientNotification(order.Buyer.Id, notification.Id);

          return Content("success");
        }
        catch (Exception ex)
        {
          return Content("error");
        }
      }
      return Content("error");
    }

    // o = OrderId
    // r = Reason
    [HttpPost]
    public ActionResult CancelOrder(string o, string r)
    {
      int orderId = int.Parse(o);
      Order order = this.Data.Orders.Find(orderId);
      if(order != null)
      {
        string sellerId = order.Seller.Id;
        User seller = this.Data.Users.Find(sellerId);
        try
        {
          seller.FinishedOrders.Add(order);
          seller.AwaitingOrders.Remove(order);

          order.Status = OrderStatus.Canceled;
          order.Item.Quantity += order.Amount;

          order.BuyerFeedback = new Feedback()
          {
            User = order.Buyer,
            Stars = 5,
            Order = order,
            Message = r != "" ? r : "Не желая повече тази поръчка."
          };
          
          this.Data.SaveChanges();
          return Content("success");
        }
        catch (Exception ex)
        {
          return Content("error");
        }
      }
      return Content("error");
    }

    [HttpPost]
    [System.Web.Mvc.Authorize(Roles = "Admin")]
    public ActionResult OrderFinished(string o)
    {
      Order order = this.Data.Orders.Find(int.Parse(o));
      if(order != null)
      {
        try
        {
          User seller = order.Seller;

          order.Status = OrderStatus.Finished;
          seller.SentOrders.Remove(order);
          seller.FinishedOrders.Add(order);

          Notification notification = CreateOrderNotification(order.Id, order.Item.Title, order.Buyer, NotificationType.FinishedOrder);
          order.Buyer.Notifications.Add(notification);
          order.Item.Sales += 1;

          this.Data.SaveChanges();

          this.SendClientNotification(order.Buyer.Id, notification.Id);
          return Content("success");
        }
        catch (Exception ex)
        {
          return Content("error");
        }
        
      }
      return HttpNotFound();
    }


    public ActionResult Finished()
    {
      IEnumerable<ConciseFinOrderViewModel> model = Mapper.Map<IEnumerable<ConciseFinOrderViewModel>>(this.UserProfile.FinishedOrders.OrderByDescending(o => o.PostedOn));
      this.ViewBag.aoCount = this.UserProfile.AwaitingOrders.Count;
      return View(model);
    }

    [HttpGet]
    public ActionResult Details(string i, string r)
    {
      int id = int.Parse(i);
      Order order = this.Data.Orders.Find(id);
      if(this.Request.IsAjaxRequest())
      {
        if(order != null)
        {
          OrderDetailsViewModel model = Mapper.Map<OrderDetailsViewModel>(order);
          return PartialView("_OrderDetails", model);
        }
        return Content("error");
      }
      return RedirectToAction("index", "account", null);
    }

    [HttpPost]
    [System.Web.Mvc.Authorize(Roles = "Admin")]
    public ActionResult SendFeedback(string o, string s, string f, string req)
    {
      int orderId = int.Parse(o);
      int stars = int.Parse(s);

      Order order = this.Data.Orders.Find(orderId);
      if(order != null)
      {        
        if(req != null && req == "seller")
        {
          try
          {
            Feedback feedback = new Feedback()
            {
              Message = (f != null && f != "") ? f : "Потребителят не е оставил отзив",
              Stars = stars,
              User = order.Seller,
              Order = order
            };

            order.SellerFeedback = feedback;
            order.Seller.GivenFeedbacks.Add(feedback);
            order.Buyer.ReceivedFeedbacks.Add(feedback);
            this.Data.SaveChanges();
            return Content("success");
          }
          catch(Exception ex)
          {
            return Content("error");
          }
        }
        else
        {
          if (order.Buyer.Id == this.User.Identity.GetUserId())
          {
            try
            {
              Feedback feedback = new Feedback()
              {
                Stars = stars,
                Message = f != null ? f : "Потребителят не е оставил отзив",
                User = order.Buyer,
                Order = order
              };

              order.Status = OrderStatus.Finished;
              order.BuyerFeedback = feedback;

              order.Seller.FinishedOrders.Add(order);
              order.Seller.SentOrders.Remove(order);

              order.Buyer.GivenFeedbacks.Add(feedback);
              order.Seller.ReceivedFeedbacks.Add(feedback);
              this.Data.SaveChanges();
              return Content("success");
            }
            catch (Exception ex)
            {
              return Content("error");
            }
          }
          return Content("error");
        }
      }
      return Content("error");
    }

    private Notification CreateOrderNotification(int objectId, string objectName, User receiver, NotificationType type)
    {
      Notification notification = new Notification()
      {
        ObjectId = objectId,
        ObjectName = objectName,
        PostedOn = DateTime.Now,
        Receiver = receiver,
        Sender = this.UserProfile,
        Type = type
      };
      return notification;
    }
  }
} 