namespace Crafty.App.Areas.Admin.Controllers
{
  using Crafty.App.Controllers;
  using Crafty.Data.UnitOfWork;
  using System.Web.Mvc;

  [Authorize(Roles = "Admin")]
  public class BaseAdminController : BaseController
  {

    public BaseAdminController(ICraftyData data)
      : base(data)
    {
    }
  }
}