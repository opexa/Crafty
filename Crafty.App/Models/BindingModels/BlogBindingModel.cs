namespace Crafty.App.Models.BindingModels
{
  using Crafty.App.Models.ViewModels;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.Web;

  public class AddBlogBindingModel
  {
    public string Title { get; set; }

    public string Content { get; set; }

    public string BlogContentIdentifier { get; set; }

    public string RelatedItems { get; set; }

    [DataType(DataType.Upload)]
    public ICollection<HttpPostedFileBase> Images { get; set; }
  }

  public class EditBlogBindingModel
  {
    [Required]
    public string Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public string LastImageId { get; set; }

    public string BlogContentIdentifier { get; set; }

    public IEnumerable<ConciseItemViewModel> CurrentRelatedItems { get; set; }

    public string UpdatedRelatedItems { get; set; }

    [DataType(DataType.Upload)]
    public ICollection<HttpPostedFileBase> NewImages { get; set; }
  }
}