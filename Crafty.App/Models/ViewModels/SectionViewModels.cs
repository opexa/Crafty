using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crafty.App.Models.ViewModels
{
  public class ConciseSectionViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
  }

  public class MenuSectionViewModel
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<MenuCategoryViewModel> Categories { get; set; }
  }

  public class DetailSectionViewModel
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<ConciseCategoryViewModel> Categories { get; set; }
  }
}