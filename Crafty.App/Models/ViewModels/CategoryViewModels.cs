using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crafty.App.Models.ViewModels
{
  public class ConciseCategoryViewModel
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public string Thumbnail { get; set; }

    public int SectionId { get; set; }
  }

  public class MenuCategoryViewModel
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public int SectionId { get; set; }

    public bool Visible { get; set; }
  }
}