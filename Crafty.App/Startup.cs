using Crafty.App.App_Start;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Crafty.App.Startup))]
namespace Crafty.App
{
  public partial class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      var idProvider = new MyUserIdProvider();
      GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);

      ConfigureAuth(app);
      app.MapSignalR();
    }
  }
}
