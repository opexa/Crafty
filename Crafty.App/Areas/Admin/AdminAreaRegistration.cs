namespace Crafty.App.Areas.Admin
{
  using System.Web.Mvc;

  public class AdminAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Admin";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "Admin_default",
          "Admin/{controller}/{action}/{id}",
          new { area = "Admin", controller = "Home", action = "Index", id = UrlParameter.Optional },
          namespaces: new[] { "Crafty.App.Areas.Admin.Controllers" }
      );
    }
  }
}