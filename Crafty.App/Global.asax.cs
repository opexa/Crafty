namespace Crafty.App
{
  using App_Start;
  using AutoMapper;
  using Controllers;
  using Microsoft.AspNet.SignalR;
  using Newtonsoft.Json;
  using System;
  using System.Web;
  using System.Web.Mvc;
  using System.Web.Optimization;
  using System.Web.Routing;

  public class MvcApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      var serializerSettings = new JsonSerializerSettings();
      serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
      serializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

      var serializer = JsonSerializer.Create(serializerSettings);
      GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);

      GlobalFilters.Filters.Add(new CssStyleMapping(), 0);
      Mapper.Initialize(c => c.AddProfile<MapperConfig>());
      AreaRegistration.RegisterAllAreas();
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
    }

    //protected void Application_Error(object sender, EventArgs e)
    //{
    //  if(Context.Response.StatusCode == 404)
    //  {
    //    Response.Clear();
    //    RouteData rd = new RouteData();
    //    rd.Values["controller"] = "Error";
    //    rd.Values["action"] = "";
    //    IController c = new ErrorController();
    //    c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
    //  }
    //  else
    //  {
    //    Response.Clear();
    //    Server.ClearError();

    //    string path = Request.Path;
    //    Context.RewritePath("~/Error", false);


    //    IHttpHandler httpHandler = new MvcHttpHandler();
    //    Context.Server.TransferRequest("~/error");
    //    Context.RewritePath(path, false);
    //  }
    //}
  }
}

