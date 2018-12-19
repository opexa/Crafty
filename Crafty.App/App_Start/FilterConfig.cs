using Crafty.App.App_Start;
using System.Web;
using System.Web.Mvc;

namespace Crafty.App
{
  public class FilterConfig
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
      filters.Add(new NoCache());
    }
  }

  public class CssStyleMapping : ActionFilterAttribute
  {
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
      string controller = filterContext.RouteData.Values["controller"].ToString().Replace("Controller", "");
      string action = filterContext.RouteData.Values["action"].ToString();

      filterContext.Controller.ViewBag.PageStyle = controller + action;
    }
  }
}
