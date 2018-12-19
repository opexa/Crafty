using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crafty.App.Models.ViewModels
{
  public class ConciseBlogViewModel
  {
    public int Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public string Thumbnail { get; set; }
  }

  public class BlogDetailsViewModel
  {

    public int Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public string Thumbnail { get; set; }

    public DateTime PostedOn { get; set; }

    public int CommentsCount { get; set; }

    public bool Visible { get; set; }

    public IEnumerable<BlogCommentViewModel> Comments { get; set; }

    public IEnumerable<BlogRelatedItemViewModel> RelatedItems { get; set; }
  }
}