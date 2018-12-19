using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crafty.App.Models.ViewModels
{
  public class BlogCommentViewModel
  {
    public int Id { get; set; }

    public string UserId { get; set; }

    public string UserName { get; set; }

    public string UserPicture { get; set; }

    public int BlogId { get; set; }

    public string Content { get; set; }
    
    public string PostedOn { get; set; }
  }
}