namespace Crafty.App.App_Start
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.Linq;
  using System.Reflection;
  using System.Web;
  using System.Web.Mvc;

  [AttributeUsage(AttributeTargets.Method)]
  public sealed class NoCache : ActionFilterAttribute
  {
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
      filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
      filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
      filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
      filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      filterContext.HttpContext.Response.Cache.SetNoStore();
    }
  }
}