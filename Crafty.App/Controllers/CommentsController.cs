namespace Crafty.App.Controllers
{
  using App_Start;
  using AutoMapper;
  using Crafty.Data.UnitOfWork;
  using Crafty.Models;
  using Hubs;
  using Microsoft.AspNet.Identity;
  using Models.ViewModels;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web.Mvc;

  public class CommentsController : BaseController
  {
    public CommentsController(ICraftyData data) : base(data) { }

    [HttpPost]
    public ActionResult AddBlogComment(string b, string c)
    {
      if (!this.User.Identity.IsAuthenticated)
        return PartialView("_LoginDialog", new LoginViewModel());

      try
      {
        Blog blog = this.Data.Blogs.Find(int.Parse(b));
        if (blog == null)
          return Content("error");

        BlogComment comment = new BlogComment
        {
          Author = this.UserProfile,
          Blog = blog,
          Content = c,
          PostedOn = DateTime.Now
        };

        Notification notification = new Notification()
        {
          ObjectId = blog.Id,
          ObjectName = blog.Title,
          PostedOn = DateTime.Now,
          Receiver = blog.Author,
          Sender = this.UserProfile,
          Type = NotificationType.BlogComment
        };
        blog.Author.Notifications.Add(notification);
        blog.Comments.Add(comment);
        this.Data.SaveChanges();
        base.SendClientNotification(blog.AuthorId, notification.Id);
        CommentsHub.RefreshBlogComments(comment);

        return Content("success");
      }
      catch(Exception ex)
      {
        return Content("error");
      }
      
      return View();
    }

    [HttpPost]
    [System.Web.Mvc.Authorize]
    public ContentResult EditBlogComment(int id, string content)
    {
      BlogComment comment = this.Data.BlogComments.Find(id);
      if (comment == null)
        return Content("not found");

      try
      {
        comment.Content = content;
        this.Data.SaveChanges();
        CommentsHub.UpdateBlogComment(content, id);
        return Content("success");
      }
      catch(Exception ex)
      {
        return Content("error");
      }
    }


    [HttpPost]
    public ContentResult DeleteBlogComment(int id)
    {
      BlogComment comment = this.Data.BlogComments.Find(id);
      if (comment == null)
        return Content("not found");

      if (comment.Author.Id != this.User.Identity.GetUserId() && !this.User.IsInRole("Admin"))
        return Content("not authorized");

      try
      {
        this.Data.BlogComments.Remove(comment);
        this.Data.SaveChanges();
        return Content("success");
      }
      catch(Exception ex)
      {
        return Content("error");
      }
    }

    [HttpGet]
    public ActionResult GetDate(string arr)
    {
      string[] ids = arr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

      var result = new List<dynamic>();

      foreach(string id in ids)
      {
        var current = this.Data.BlogComments.Find(int.Parse(id));
        result.Add(new
        {
          Id = current.Id,
          Date = current.PostedOn.ToRelativeDateString()
        });
      }
      return Json(new { data = result }, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    public ActionResult BlogComments(int id, string p)
    {
      try
      {
        Blog blog = this.Data.Blogs.Find(id);
        IEnumerable<BlogCommentViewModel> model = Mapper.Map<IEnumerable<BlogCommentViewModel>>(blog.Comments.OrderByDescending(c => c.PostedOn)
                                                                                                             .Skip(int.Parse(p) * 15).Take(15));

        return PartialView("_Comments", model);
      }
      catch(Exception ex)
      {
        return Content("error");
      }
    }
  }
}