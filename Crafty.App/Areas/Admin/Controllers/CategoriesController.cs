namespace Crafty.App.Areas.Admin.Controllers
{
  using AutoMapper;
  using Crafty.App.Models.ViewModels;
  using Crafty.Data.UnitOfWork;
  using System.Collections.Generic;
  using System.Web.Mvc;

  public class CategoriesController : BaseAdminController
  {
    public CategoriesController(ICraftyData data) : base(data) { }

    
    public ActionResult Index()
    {
      var sections = this.Data.Sections.All();

      return View(Mapper.Map<IEnumerable<ConciseSectionViewModel>>(sections));
    }
  }
}