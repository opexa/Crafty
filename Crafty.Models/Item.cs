namespace Crafty.Models
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.ComponentModel.DataAnnotations;

  public class Item
  {
    public Item()
    {
      this.Likes = new HashSet<Like>();
    }

    [Key]
    public int Id { get; set; }
    
    [Timestamp]
    public byte[] RowVersion { get; set; }

    [Required]
    public string Title { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    [Required]
    public DateTime PostedOn { get; set; }
    
    public virtual User Seller { get; set; }

    [Required]
    public int SellerId { get; set; }
    
    public decimal? Price { get; set; }

    [Required]
    [Range(1, 9999999)]
    public int Quantity { get; set; }

    public int Sales { get; set; }

    public string Thumbnail { get; set; }

    [MaxLength(1000)]
    public string Pictures { get; set; }

    [DefaultValue(0)]
    public int Views { get; set; }
    
    public virtual Category Category { get; set; }

    public virtual ICollection<Like> Likes { get; set; }
  }
}
