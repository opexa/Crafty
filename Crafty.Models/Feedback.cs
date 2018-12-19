namespace Crafty.Models
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  public class Feedback
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public virtual Order Order { get; set; }

    [Required]
    public virtual User User { get; set; }
    
    [MaxLength(200)]
    public string Message { get; set; }

    [Range(0, 6)]
    public int Stars { get; set; }
  }
}
