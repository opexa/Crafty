using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Crafty.App
{
  public class RouteConfig
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      //routes.MapRoute(
      //  name: "UsernameAvailabilityCheck",
      //  url: "account/checkname/{username}",
      //  defaults: new { controller = "Account", action = "Checkname", username = UrlParameter.Optional },
      //  namespaces: new[] { "Crafty.App.Controllers" }
      //);

      routes.MapRoute(
          name: "UserProfile",
          url: "account/users/{username}",
          defaults: new { controller = "Account", action = "Users", username = UrlParameter.Optional },
          namespaces: new[] { "Crafty.App.Controllers" }
      );

      routes.MapRoute(
        name: "SearchRoute",
        url: "search/word/{target}",
        defaults: new { controller = "Search", action = "Word", target = UrlParameter.Optional },
        namespaces: new[] { "Crafty.App.Controllers" }
      );

      routes.MapRoute(
          name: "Default",
          url: "{controller}/{action}/{id}",
          defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
          namespaces: new[] { "Crafty.App.Controllers" }
      );

      //AreaRegistration.RegisterAllAreas();
    }
  }
}
