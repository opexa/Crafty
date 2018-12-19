namespace Crafty.Models
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public class Blog
  {
    public Blog()
    {
      this.RelatedItems = new HashSet<Item>();
      this.Comments = new HashSet<BlogComment>();
    }

    [Key]
    public int Id { get; set; }

    [Required]
    [MinLength(5)]
    public string Title { get; set; }
   
    public virtual User Author { get; set; }

    public string AuthorId { get; set; }

    public string Thumbnail { get; set; }

    [Required]
    [MinLength(30)]
    public string Content { get; set; }

    public string BlogContentIdentifier { get; set; }

    public int LastImageId { get; set; }

    public DateTime PostedOn { get; set; }
    
    public bool Visible { get; set; }

    public int Views { get; set; }

    public virtual ICollection<Item> RelatedItems { get; set; }

    public virtual ICollection<BlogComment> Comments { get; set; }
  }
}
