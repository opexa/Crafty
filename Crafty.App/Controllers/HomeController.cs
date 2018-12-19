namespace Crafty.App.Controllers
{
  using AutoMapper;
  using Crafty.Models;
  using Models.ViewModels;
  using Crafty.Data.UnitOfWork;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web.Mvc;

  public class HomeController : BaseController
  {
    public HomeController(ICraftyData data) : base(data) { }

    public ActionResult Index()
    {
      HomeViewModelBag model = new HomeViewModelBag();


      IQueryable<Item> items = this.Data.Items.All()
        .OrderByDescending(o => o.Views).ThenBy(o => o.PostedOn).Take(15);

      model.PopularItems = Mapper.Map<IEnumerable<ConciseItemViewModel>>(items);
      model.PopularCategories = this.GetPopularCategories();
      model.Blogs = this.GetLatestBlogs();
      return View(model);
    }

    private IEnumerable<ConciseBlogViewModel> GetLatestBlogs()
    {
      IQueryable<Blog> blogs = this.Data.Blogs.All().Where(b => b.Visible == true).OrderByDescending(b => b.PostedOn).Take(5);
      return Mapper.Map<IEnumerable<ConciseBlogViewModel>>(blogs);
    }

    private IEnumerable<ConciseCategoryViewModel> GetPopularCategories()
    {
      IEnumerable<Category> categories = this.Data.Categories.All();
      var categoriesObject = new List<RawCategory>();

      foreach(Category category in categories)
      {
        var topItems = category.Items.OrderByDescending(i => i.Views).Take(10);
        int itemsViewsCount = 0;
        foreach(Item item in topItems)
        {
          itemsViewsCount += item.Views;
        }
        categoriesObject.Add(new RawCategory
        {
          ItemsViewsCount = itemsViewsCount,
          Category = category
        });
      }

      var rawModel = categoriesObject.ToList().OrderByDescending(c => c.ItemsViewsCount).Take(3).Select(c => c.Category);

      IEnumerable<ConciseCategoryViewModel> model = Mapper.Map<IEnumerable<ConciseCategoryViewModel>>(rawModel);

      return model;
    }
  }

  internal class RawCategory
  {
    public Category Category { get; set; }

    public int ItemsViewsCount { get; set; }

  }
}