namespace Crafty.Models
{
  using Crafty.Models;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.ComponentModel.DataAnnotations;

  public class Category
  {
    public Category()
    {
      this.Items = new HashSet<Item>();
    }

    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string Name { get; set; }

    [DefaultValue("/images/trash/fd.jpg")]
    public string Thumbnail { get; set; }

    public bool Visible { get; set; }

    public virtual Section Section { get; set; }

    public virtual ICollection<Item> Items { get; set; }
  }
}
