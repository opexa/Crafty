namespace Crafty.App.Controllers
{
  using AutoMapper;
  using Crafty.Data.UnitOfWork;
  using Crafty.Models;
  using Models.ViewModels;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web.Mvc;

  public class SearchController : BaseController
  {
    public SearchController(ICraftyData data) : base(data) { }
    
    // GET: Search
    public ActionResult Word(string target)
    {
      this.ViewBag.TargetWord = target;
      return View(GetSuggestedItems(target));
    }

    [HttpGet]
    public ActionResult RelatedItems(string title)
    {
      IEnumerable<ConciseItemViewModel> model = this.GetSuggestedItems(title);
      return Json(model, JsonRequestBehavior.AllowGet);
    }
    private IEnumerable<ConciseItemViewModel> GetItemsWithSimilarTitle(string target)
    {
      string[] words = target.Split(new[] { ' ' });

      List<ConciseItemViewModel> model = new List<ConciseItemViewModel>();

      foreach(string word in words)
      {
        IEnumerable<Item> items = this.Data.Items.All().Where(i => i.Title.Contains(word));
        foreach(Item item in items)
        {
          model.Add(Mapper.Map<ConciseItemViewModel>(item));
        }
      }
      return model;
    }

    private IEnumerable<ConciseItemViewModel> GetSuggestedItems(string target)
    {
      List<string> searchStrings = new List<string>();

      List<string> targetParams = target.Split(new[] { ' ' }).ToList();
      int length = targetParams.Count;
      for(int i = 0; i < length; i++)
      {
        string sentence = "";
        for(int j = 0; j < i + 1; j++)
        {
          sentence += targetParams[j] + " ";
        }
        
        if (!searchStrings.Contains(sentence))
          searchStrings.Add(sentence);
      }

      List<Item> model = new List<Item>();

      foreach(string searchString in searchStrings)
      {
        IEnumerable<Item> items = this.Data.Items.All().Where(i => i.Description.Contains(searchString) || i.Title.Contains(searchString));
        foreach(Item item in items)
        {
          if(!model.Contains(item))
          {
            model.Add(item);
          }
        }
      }      
      return Mapper.Map<IEnumerable<ConciseItemViewModel>>(model.Distinct().ToList());
    }

    // TODO: Add method wich searches for articles/blog posts which contain the target of the search
  }
}