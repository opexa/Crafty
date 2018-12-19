namespace Crafty.Models
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public class BlogComment
  {
    [Key]
    public int Id { get; set; }

    public virtual User Author { get; set; }

    [Required]
    public string UserId { get; set; }

    public virtual Blog Blog { get; set; }

    [Required]
    public int BlogId { get; set; }

    public string Content { get; set; }

    public DateTime PostedOn { get; set; }
  }
}
