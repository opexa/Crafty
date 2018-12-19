using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crafty.App.Controllers
{
  using AutoMapper;
  using Crafty.Models;
  using Data.UnitOfWork;
  using Models.ViewModels;
  using System.Web.Mvc;

  [Authorize]
  public class NotificationsController : BaseController
  {
    public NotificationsController(ICraftyData data) : base(data) { }

    [HttpGet]
    public ActionResult All(int? id)
    {
      int skip = id != null ? int.Parse(id.ToString()) : 0;

        IEnumerable<ConciseNotificationViewModel> model = Mapper.Map<IEnumerable<ConciseNotificationViewModel>>
                                                          (this.UserProfile.Notifications.OrderByDescending(o => o.PostedOn).Skip(skip).Take(7));
        return PartialView("All", model);
    }

    [HttpGet]
    public ActionResult Get(int id)
    {
      Notification notif = this.Data.Notifications.Find(id);
      if(notif != null)
      {
        ConciseNotificationViewModel model = Mapper.Map<ConciseNotificationViewModel>(notif);
        return PartialView("~/Views/Shared/DisplayTemplates/ConciseNotificationViewModel.cshtml", model);
      }
      return HttpNotFound("Възникна грешка");
    }

    [HttpPost]
    public ActionResult AllRead(string go)
    {
      if(go == "true")
      {
        IEnumerable<Notification> notifications = this.UserProfile.Notifications.Where(n => n.Seen == false);
        try
        {
          foreach (Notification notif in notifications)
          {
            notif.Seen = true;
          }
          this.Data.SaveChanges();
          return Content("Success");
        }
        catch (Exception ex)
        {
          return HttpNotFound();
        }
      }
      return Content("Error");
    }

    [HttpPost]
    public ActionResult MarkRead(string i)
    {
      Notification notification = this.Data.Notifications.Find(int.Parse(i));
      if(notification != null)
      {
        try
        {
          notification.Seen = true;
          this.Data.SaveChanges();
          return Content("1");
        }
        catch(Exception ex)
        {
          return Content("0");
        }
      }
      return Content("0");
    }
  }
}