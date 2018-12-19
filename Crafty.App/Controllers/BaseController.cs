namespace Crafty.App.Controllers
{
  using Crafty.Models;
  using Crafty.Data.UnitOfWork;
  using System;
  using System.Linq;
  using System.Web.Mvc;
  using System.Web.Routing;
  using Microsoft.AspNet.Identity;
  using Hubs;
  using Microsoft.AspNet.SignalR;
  using AutoMapper;
  using System.Collections.Generic;
  using Models.ViewModels;
  using System.Web.UI;

  public class BaseController : Controller
  {
    private ICraftyData data;
    private User userProfile;

    protected BaseController(ICraftyData data)
    {
      this.Data = data;
    }

    protected BaseController(ICraftyData data, User userProfile)
      : this(data)
    {
      this.UserProfile = userProfile;
    }

    protected void SendClientNotification(string receiver, int notifId)
    {
      var hub = GlobalHost.ConnectionManager.GetHubContext<NotificationsHub>();
      hub.Clients.User(receiver).receiveNotification(notifId);
    }

    protected ICraftyData Data { get; private set; }

    protected User UserProfile { get; private set; }

    protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
    {
      if(requestContext.HttpContext.User.Identity.IsAuthenticated)
      {
        var userId = requestContext.HttpContext.User.Identity.GetUserId();
        var user = this.Data.Users.Find(userId);
        this.UserProfile = user;

        this.ViewBag.Notifications = user.Notifications.Any() ? user.Notifications.Count(n => n.Seen == false) : 0;
      }
      return base.BeginExecute(requestContext, callback, state);
    }

    //protected override void OnException(ExceptionContext filterContext)
    //{
    //  filterContext.ExceptionHandled = true;
    //  filterContext.Result = new ViewResult() { ViewName = "/error" };
    //  base.OnException(filterContext);
    //}

    protected override void Initialize(RequestContext requestContext)
    {
      this.PassSectionsToView();
      base.Initialize(requestContext);
    }

    //[OutputCache(Duration = 24*60*60*365, Location = OutputCacheLocation.Client)]
    public void PassSectionsToView()
    {
      var list = this.Data.Sections.All();

      this.ViewBag.DropdownSections = Mapper.Map<IEnumerable<MenuSectionViewModel>>(list);
    }
  }
}