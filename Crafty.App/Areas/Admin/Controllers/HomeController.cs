using Crafty.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Crafty.App.Areas.Admin.Controllers
{
  public class HomeController : BaseAdminController
  {
    public HomeController(ICraftyData data) : base(data) { }


    public ActionResult Index()
    {
      return View();
    }
  }
}